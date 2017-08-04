using System;
using System.Collections.Generic;
using System.Linq;

namespace WebsiteProvider.Api
{
    public partial class PinningHandlers
    {
        private class PinningRulesMappingCollection
        {
            private readonly object locker = new object();
            private readonly List<PinningRuleMappingInfo> mappingInfos = new List<PinningRuleMappingInfo>();

            public void Add(PinningRuleMappingInfo info)
            {
                lock (locker)
                {
                    this.mappingInfos.Add(info);
                }
            }

            public List<PinningRuleMappingInfo> Take(Func<PinningRuleMappingInfo, bool> predicate)
            {
                lock (locker)
                {
                    var items = this.mappingInfos.Where(predicate).ToList();
                    foreach (var info in items)
                    {
                        if (info.MapIds.Count <= 1)
                        {
                            this.mappingInfos.Remove(info);
                        }
                    }
                    return items;
                }
            }

            public void Process(Func<PinningRuleMappingInfo, bool> predicate, Action<List<PinningRuleMappingInfo>> action)
            {
                lock (locker)
                {
                    var items = this.mappingInfos.Where(predicate).ToList();
                    action(items);
                }
            }
        }
    }
}
