using System;
using System.Collections.Generic;
using System.Text;

namespace Vre.Server.Mls
{
    public class RetsMlsStatusProvider : RetsMlsInfoProviderBase
    {
        private int _mlsNumIdx = -1;
        private List<string> _items = new List<string>();

        public override IList<string> GetCurrentActiveItems() { return _items; }
        public override IList<MlsItem> GetNewItems() { throw new NotImplementedException(); }

        protected override void syncUp(string[] header)
        {
            for (int idx = header.Length - 1; idx >= 0; idx--)
            {
                if (header[idx].Equals("Ml_num")) _mlsNumIdx = idx;
            }

            if (_mlsNumIdx < 0)
                throw new ApplicationException("Sync file has no header or header is invalid");
        }

        protected override void process(string[] row, StringBuilder warnings)
        {
            if (row.Length > _mlsNumIdx) _items.Add(row[_mlsNumIdx]);
            else throw new ApplicationException("Sync file has invalid content");
        }
    }
}