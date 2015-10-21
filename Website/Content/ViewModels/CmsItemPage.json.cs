using System;
using System.IO;
using Starcounter;

namespace Content {
    partial class CmsItemPage : Page, IBound<ContentItem> {
        protected override void OnData() {
            base.OnData();

            string path = this.GetFilePath();
            FileInfo fi = new FileInfo(path);

            if (fi.Exists) {
                TextReader reader = new StreamReader(fi.FullName);

                this.HtmlValue = reader.ReadToEnd();
                reader.Dispose();
            } else {
                this.HtmlValue = null;
            }
        }

        void Handle(Input.SaveClick Action) {
            string path = this.GetFilePath();
            FileInfo fi = new FileInfo(path);

            if (fi.Exists) {
                fi.Delete();
            }

            TextWriter writer = new StreamWriter(fi.FullName, false, System.Text.Encoding.UTF8);

            writer.Write(this.HtmlValue);
            writer.Dispose();

            this.Transaction.Commit();
            this.Message = "The changes has been successfully saved!";
        }

        protected string GetFilePath() {
            return Starcounter.Application.Current.WorkingDirectory + DataHelper.UrlPrefix + this.HtmlPath;
        }
    }
}
