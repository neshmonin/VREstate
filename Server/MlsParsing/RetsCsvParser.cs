using System.Text;
using System.Collections.Generic;
using System.IO;
using System;

namespace Vre.Server.Mls
{
    public class RetsCsvParser
    {
        private StringBuilder _readWarnings;
        private List<MlsItem> _units;

        public IEnumerable<MlsItem> Units { get { return _units; } }

        public string Parse(string fileName)
        {
            _readWarnings = new StringBuilder();
            _units = new List<MlsItem>();

            try
            {
                using (FileStream file = File.OpenRead(fileName))
                {
                    using (StreamReader rdr = new StreamReader(file))
                    {
                        syncUp(CsvUtilities.Split(rdr.ReadLine()));

                        while (!rdr.EndOfStream) process(CsvUtilities.Split(rdr.ReadLine()));
                    }
                }
            }
            catch (Exception e)
            {
                _readWarnings.AppendFormat("Fatal error: {0}\r\n{1}\r\n", e.Message, e.StackTrace);
            }

            string result = _readWarnings.ToString();
            _readWarnings.Length = 0;
            return result;
        }

        private int _listPriceIdx = -1;
        private int _mlsNumIdx = -1;
        private int _saleLeaseIdx = -1;
        private int _statusIdx = -1;
        private int _vTourUrlIdx = -1;
        private int _vTourUpdateIdx = -1;

        private void syncUp(string[] header)
        {
            for (int idx = header.Length - 1; idx >= 0; idx--)
            {
                if (header[idx].Equals("Lp_dol")) _listPriceIdx = idx;
                else if (header[idx].Equals("Vtour_updt")) _vTourUpdateIdx = idx;
                //else if (header[idx].Equals("Zip")) _listPriceIdx = idx;
                //else if (header[idx].Equals("Unit_num")) _listPriceIdx = idx;
                //else if (header[idx].Equals("Timestamp_sql")) _listPriceIdx = idx;
                else if (header[idx].Equals("Tour_url")) _vTourUrlIdx = idx;
                //else if (header[idx].Equals("Sqft")) _listPriceIdx = idx;
                //else if (header[idx].Equals("St")) _listPriceIdx = idx;
                //else if (header[idx].Equals("St_dir")) _listPriceIdx = idx;
                //else if (header[idx].Equals("St_num")) _listPriceIdx = idx;
                //else if (header[idx].Equals("St_sfx")) _listPriceIdx = idx;
                else if (header[idx].Equals("Status")) _statusIdx = idx;
                //else if (header[idx].Equals("Stories")) _listPriceIdx = idx;
                //else if (header[idx].Equals("Pix_updt")) _listPriceIdx = idx;
                else if (header[idx].Equals("Ml_num")) _mlsNumIdx = idx;
                //else if (header[idx].Equals("Municipality")) _listPriceIdx = idx;
                //else if (header[idx].Equals("Idx_dt")) _listPriceIdx = idx;
                //else if (header[idx].Equals("Apt_num")) _listPriceIdx = idx;
                //else if (header[idx].Equals("County")) _listPriceIdx = idx;
                //else if (header[idx].Equals("Disp_addr")) _listPriceIdx = idx;
                //else if (header[idx].Equals("Type_own_srch")) _listPriceIdx = idx;
                else if (header[idx].Equals("S_r")) _saleLeaseIdx = idx;
            }
        }

        private void process(string[] row)
        {
            string mls = null;
            double prc = 0.0;
            MlsItem.SaleLease sl = MlsItem.SaleLease.Unknown;

            if ((_mlsNumIdx >= 0) && (row.Length > _mlsNumIdx)) mls = row[_mlsNumIdx];

            if ((_listPriceIdx >= 0) && (row.Length > _listPriceIdx))
            {
                double val;
                if (double.TryParse(row[_listPriceIdx].Trim(), out val)) prc = val;
            }

            if ((_saleLeaseIdx >= 0) && (row.Length > _saleLeaseIdx))
            {
                string sls = row[_saleLeaseIdx];
                if (sls.Equals("Sale")) sl = MlsItem.SaleLease.Sale;
                else if (sls.Equals("Lease")) sl = MlsItem.SaleLease.Lease;
                else _readWarnings.AppendFormat("MLS {0}: Unknown SaleLease type: {1}\r\n", mls, sls);
            }

            if ((_statusIdx >= 0) && (row.Length > _statusIdx))
            {
                if (!row[_statusIdx].Equals("A"))
                    _readWarnings.AppendFormat("MLS {0}: Unknown status: {1}\r\n", mls, row[_statusIdx]);
            }

            mls.Equals(sl);
            mls.Equals(prc);
        }
    }
}