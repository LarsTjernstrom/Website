using System;
using System.Linq;
using Simplified.Ring6;
using Starcounter;
using WebsiteEditor.Helpers;

namespace WebsiteEditor.ViewModels
{
    partial class CatchingRulesPage : Json, IKnowSurfacePage
    {
        public string SurfaceKey { get; set; }

        public void RefreshData()
        {
            if (string.IsNullOrEmpty(this.SurfaceKey))
            {
                throw new InvalidOperationException("Surface key is empty.");
            }

            this.CatchingRules.Clear();
            this.SurfaceName = this.GetCurrentSurface().Name;
            this.CatchingRules.Data = Db.SQL<WebUrl>("SELECT u FROM Simplified.Ring6.WebUrl u WHERE u.Template.Key = ? ORDER BY u.Template.Name, u.Url", this.SurfaceKey);
            foreach (var catchingRule in this.CatchingRules)
            {
                catchingRule.DeleteAction = this.DeleteCatchingRule;
            }
            this.Trn.Data = this.Transaction as Transaction;
        }

        void Handle(Input.CancelChangesTrigger action)
        {
            this.Transaction.Rollback();
            this.RefreshData();
        }

        void Handle(Input.SaveChangesTrigger action)
        {
            foreach (var catchingRule in this.CatchingRules)
            {
                var emptyHeaders = catchingRule.Headers.Where(h => string.IsNullOrWhiteSpace(h.Name)).ToList();
                foreach (var catchHeader in emptyHeaders)
                {
                    catchingRule.DeleteHeader(catchHeader);
                }
            }
            this.Transaction.Commit();
        }

        void Handle(Input.CreateTrigger action)
        {
            var surface = this.GetCurrentSurface();
            this.CatchingRules.Add(new CatchingRulesItemPage
            {
                Data = new WebUrl
                {
                    Template = surface,
                    Url = string.Empty,
                },
                DeleteAction = this.DeleteCatchingRule
            });
        }

        public void DeleteCatchingRule(CatchingRulesItemPage catchingRule)
        {
            this.CatchingRules.Remove(catchingRule);
            catchingRule.Data.Delete();
        }

        private WebTemplate GetCurrentSurface()
        {
            return Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t WHERE t.Key = ?", this.SurfaceKey).FirstOrDefault()
                   ?? throw new Exception("The surface with specified key is not found.");
        }


        [CatchingRulesPage_json.CatchingRules]
        partial class CatchingRulesItemPage : Json, IBound<WebUrl>
        {
            public Action<CatchingRulesItemPage> DeleteAction { get; set; }

            protected override void OnData()
            {
                base.OnData();
                this.UpdateHeaderString();

                foreach (var header in this.Headers)
                {
                    header.DeleteAction = this.DeleteHeader;
                }
            }

            void Handle(Input.DeleteTrigger action)
            {
                this.DeleteAction?.Invoke(this);
            }

            void Handle(Input.AddHeaderTrigger action)
            {
                this.Headers.Add(new CatchHeadersItemPage
                {
                    Data = new WebHttpHeader { Url = this.Data },
                    DeleteAction = this.DeleteHeader
                });
            }

            public void DeleteHeader(CatchHeadersItemPage header)
            {
                this.Headers.Remove(header);
                header.Data.Delete();
            }

            public void UpdateHeaderString()
            {
                if (this.Headers.Any())
                {
                    var firstHeader = this.Headers.First();
                    this.HeadersString = $"{firstHeader.Name}: {firstHeader.Value}";
                    if (this.Headers.Count > 1)
                    {
                        this.HeadersString += $" + {this.Headers.Count - 1} other headers";
                    }
                }
                else
                {
                    this.HeadersString = "Any HTTP headers";
                }
            }
        }

        [CatchingRulesPage_json.CatchingRules.Headers]
        partial class CatchHeadersItemPage : Json, IBound<WebHttpHeader>
        {
            private CatchingRulesPage_json.CatchingRules ParentPage => this.Parent.Parent as CatchingRulesPage_json.CatchingRules; // <========== Here

            public Action<CatchHeadersItemPage> DeleteAction { get; set; }

            protected override void OnData()
            {
                base.OnData();


            }

            void Handle(Input.DeleteTrigger action)
            {
                this.DeleteAction?.Invoke(this);
            }
        }

        [CatchingRulesPage_json.Trn]
        partial class CatchingRulesTransactionPage : Json, IBound<Transaction>
        {
        }
    }
}
