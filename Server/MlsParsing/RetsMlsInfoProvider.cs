using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Vre.Server.Mls
{
    public class RetsMlsInfoProvider : RetsMlsInfoProviderBase
    {
        private int _rowLen = 0;

        private int _mlsNumIdx = -1;
        private int _statusIdx = -1;
        private int _listingTypeIdx = -1;
        private int _timestampIdx = -1;

        private int _postalCodeIdx = -1;
        private int _stateProvinceIdx = -1;
        private int _municipalityIdx = -1;
		private int _streetNumberIdx = -1;
		private int _streetNameIdx = -1;
		private int _streetDirIdx = -1;
		private int _streetTypeIdx = -1;
        private List<int> _roomTypeIdx = new List<int>();
        private List<int> _washroomTypeIdx = new List<int>();
        private int _unitNumberIdx = -1;
        private int _apartmentNumberIdx = -1;

        private int _priceIdx = -1;

        private int _vtourUrlIdx = -1;

        private int _roomCnt1Idx = -1;
        private int _roomCnt2Idx = -1;
        private int _bedroomCnt1Idx = -1;
        private int _bedroomCnt2Idx = -1;
        private int _bathroomCntIdx = -1;
        private int _floorAreaRangeIdx = -1;

        private List<MlsItem> _items = new List<MlsItem>();

        public override IList<string> GetCurrentActiveItems() { throw new NotImplementedException(); }
        public override IList<MlsItem> GetNewItems() { return _items; }

        protected override void syncUp(string[] header)
        {
            // Lp_dol,"Vtour_updt","Wcloset_t1","Wcloset_t2","Wcloset_t3","Wcloset_t4","Wcloset_t5",Zip,
            //"Unit_num","Timestamp_sql",Tour_url,"Sqft","St","St_dir","St_num","St_sfx","Status","Stories",
            //Rms,Rooms_plus,S_r,"Rm1_out","Rm10_out","Rm11_out","Rm12_out","Rm2_out","Rm3_out","Rm4_out","Rm5_out",
            //"Rm6_out","Rm7_out","Rm8_out","Rm9_out","Rltr","Pix_updt","Patio_ter","Num_kit",Ml_num,"Municipality",
            //Level1,Level10,Level11,Level12,Level2,Level3,Level4,Level5,Level6,Level7,Level8,Level9,"Idx_dt",
            //"Kit_plus",Den_fr,Br,Br_plus,Addr,"Apt_num","Bath_tot","Bsmt1_out","Bsmt2_out",Corp_num,"County",
            //Disp_addr,Type_own_srch

            for (int idx = header.Length - 1; idx >= 0; idx--)
            {
                if (header[idx].Equals("Ml_num")) _mlsNumIdx = idx;
                if (header[idx].Equals("Status")) _statusIdx = idx;  // must be "A"?
                if (header[idx].Equals("S_r")) _listingTypeIdx = idx;  // "Sale" or "Lease"
                if (header[idx].Equals("Timestamp_sql")) _timestampIdx = idx;  // yyyy-MM-dd HH:mm:ss.f America/Toronto

                // Rltr - Realtor name
                // Pix_updt - Realtor update timestamp? can be old yyyy-MM-dd HH:mm:ss.f America/Toronto

                // Idx_dt - another timestamp?! can be old/empty yyyy-MM-dd HH:mm:ss.f America/Toronto

                // Corp_num - ingeter, can be 0

                // Disp_addr - "Y"
                // Type_own_srch - "C." | "T." | "2." | ...

                if (header[idx].Equals("Zip")) _postalCodeIdx = idx;
                if (header[idx].Equals("County")) _stateProvinceIdx = idx;
                if (header[idx].Equals("Municipality")) _municipalityIdx = idx;
                if (header[idx].Equals("St_num")) _streetNumberIdx = idx;
                if (header[idx].Equals("St")) _streetNameIdx = idx;
                if (header[idx].Equals("St_sfx")) _streetTypeIdx = idx;
                if (header[idx].Equals("St_dir")) _streetDirIdx = idx;
                if (header[idx].Equals("Apt_num")) _apartmentNumberIdx = idx;
                if (header[idx].Equals("Unit_num")) _unitNumberIdx = idx;  // ?!?!?!
                // Addr - compiled street address

                if (header[idx].Equals("Lp_dol")) _priceIdx = idx;

                if (header[idx].Equals("Tour_url")) _vtourUrlIdx = idx;
                // Vtour_updt - Virtual Tour upload timestamp? can be old/empty yyyy-MM-dd HH:mm:ss.f America/Toronto

                if (header[idx].Equals("Rms")) _roomCnt1Idx = idx;  // integer room count
                if (header[idx].Equals("Rooms_plus")) _roomCnt2Idx = idx;  // integer extra room count or empty
                // Num_kit - integer kitchen count, 0+
                // Kit_plus - integer 1+ or empty
                // Den_fr - Family Room - "Y" | "N"
                if (header[idx].Equals("Br")) _bedroomCnt1Idx = idx;  // ingeger 0+
                if (header[idx].Equals("Br_plus")) _bedroomCnt2Idx = idx;  // integer 1+ or empty
                if (header[idx].Equals("Bath_tot")) _bathroomCntIdx = idx;  // integer 1+ or empty
                // Bsmt1_out - "None" | ...
                // Bsmt2_out - empty
                if (header[idx].Equals("Wcloset_t1")) _washroomTypeIdx.Add(idx);  // integer washroom types 1-2 or empty
                if (header[idx].Equals("Wcloset_t2")) _washroomTypeIdx.Add(idx);
                if (header[idx].Equals("Wcloset_t3")) _washroomTypeIdx.Add(idx);
                if (header[idx].Equals("Wcloset_t4")) _washroomTypeIdx.Add(idx);
                if (header[idx].Equals("Wcloset_t5")) _washroomTypeIdx.Add(idx);
                if (header[idx].Equals("Rm1_out")) _roomTypeIdx.Add(idx);  // text type of room or empty
                if (header[idx].Equals("Rm2_out")) _roomTypeIdx.Add(idx);
                if (header[idx].Equals("Rm3_out")) _roomTypeIdx.Add(idx);
                if (header[idx].Equals("Rm4_out")) _roomTypeIdx.Add(idx);
                if (header[idx].Equals("Rm5_out")) _roomTypeIdx.Add(idx);
                if (header[idx].Equals("Rm6_out")) _roomTypeIdx.Add(idx);
                if (header[idx].Equals("Rm7_out")) _roomTypeIdx.Add(idx);
                if (header[idx].Equals("Rm8_out")) _roomTypeIdx.Add(idx);
                if (header[idx].Equals("Rm9_out")) _roomTypeIdx.Add(idx);
                if (header[idx].Equals("Rm10_out")) _roomTypeIdx.Add(idx);
                if (header[idx].Equals("Rm11_out")) _roomTypeIdx.Add(idx);
                if (header[idx].Equals("Rm12_out")) _roomTypeIdx.Add(idx);
                // Patio_ter - "Open" | "Terr" | "None" | ...
                // Level1... Level12 - "" | "Main" | "Flat" | ...
                // Stories - integer building stories count
                if (header[idx].Equals("Sqft")) _floorAreaRangeIdx = idx;  // this is integer range: <min value>-<max value>
            }            

            if ((_mlsNumIdx < 0) || (_priceIdx < 0) || (_listingTypeIdx < 0))
                throw new ApplicationException("Sync file has no header or header is invalid");

            _rowLen = header.Length;
        }

        protected override void process(string[] row, StringBuilder warnings)
        {
            if (row.Length != _rowLen) throw new ApplicationException("Sync file has invalid content");

            MlsItem item = new MlsItem();

            item.MlsId = row[_mlsNumIdx];  // this item must exist

            item.CurrentPrice = double.Parse(row[_priceIdx]);

	        string listringType = row[_listingTypeIdx];
			if (listringType.Equals("Sale")) item.SaleLeaseState = MlsItem.SaleLease.Sale;
			else if (listringType.Equals("Lease")) item.SaleLeaseState = MlsItem.SaleLease.Lease;
			else item.SaleLeaseState = MlsItem.SaleLease.Unknown;

            if (_vtourUrlIdx >= 0) item.VTourUrl = row[_vtourUrlIdx].Trim();

            if (_postalCodeIdx >= 0) item.PostalCode = row[_postalCodeIdx];
            if (_stateProvinceIdx >= 0) item.StateProvince = row[_stateProvinceIdx];
            if (_municipalityIdx >= 0) item.Municipality = row[_municipalityIdx];
			if (_streetNumberIdx >= 0) item.StreetNumber = row[_streetNumberIdx];
			if (_streetNameIdx >= 0) item.StreetName = row[_streetNameIdx];
			if (_streetTypeIdx >= 0) item.StreetType = row[_streetTypeIdx];
			if (_streetDirIdx >= 0) item.StreetDirection = row[_streetDirIdx];
            if (_apartmentNumberIdx >= 0) item.SuiteName = row[_apartmentNumberIdx];

			// Street type conversion
			//
	        if (!string.IsNullOrEmpty(item.StreetType))
	        {
		        if (item.StreetType.Equals("Crt")) item.StreetType = "Ct";  // Court is spelled differently in our system
	        }

			// Province name conversion
			//
	        if (!string.IsNullOrEmpty(item.StateProvince))
	        {
		        if (item.StateProvince.Equals("Ontario")) item.StateProvince = "ON";

			}

	        var cai = false;
			var ca = new StringBuilder();
			if (!string.IsNullOrEmpty(item.SuiteName)) { ca.AppendFormat("#{0}", item.SuiteName); cai = true; }
			if (!string.IsNullOrEmpty(item.StreetNumber)) { if (cai) ca.Append(" - "); ca.Append(item.StreetNumber); cai = true; }
			if (!string.IsNullOrEmpty(item.StreetName)) { if (cai) ca.Append(" "); ca.Append(item.StreetName); cai = true; }
			if (!string.IsNullOrEmpty(item.StreetType)) { if (cai) ca.Append(" "); ca.Append(item.StreetType); cai = true; }
			if (!string.IsNullOrEmpty(item.StreetDirection)) { if (cai) ca.Append(" "); ca.Append(item.StreetDirection); cai = true; }
			if (!string.IsNullOrEmpty(item.Municipality)) { if (cai) ca.Append(", "); ca.Append(item.Municipality); cai = true; }
			if (!string.IsNullOrEmpty(item.StateProvince)) { if (cai) ca.Append(", "); ca.Append(item.StateProvince); cai = true; }
			if (!string.IsNullOrEmpty(item.PostalCode)) { if (cai) ca.Append(", "); ca.Append(item.PostalCode); cai = true; }
	        item.CompiledAddress = ca.ToString();

#if DEBUG
            int rc0 = getRoomCount(row, warnings);
            int rc1 = getRoomCountAlt(row, warnings);
            int bd = getBedroomCount(row, warnings);
            int bt0 = getBathroomCount(row, warnings);
            int bt1 = getBathroomCountAlt(row, warnings);
            rc0.Equals(rc1).Equals(bd).Equals(bt0).Equals(bt1);
#endif

            _items.Add(item);
        }

		//private string buildStreetAddress(string[] row)
		//{
		//    StringBuilder address = new StringBuilder();
		//    string space = string.Empty;

		//    foreach (int idx in _addressIdx)
		//    {
		//        string e = row[idx].Trim();
		//        if (e.Length > 0)
		//        {
		//            address.Append(space);
		//            address.Append(e); 
		//            space = " ";
		//        }
		//    }

		//    return (address.Length > 0) ? address.ToString() : null;
		//}

        private int getRoomCount(string[] row, StringBuilder warnings)
        {
            int result = 0;
            string field;
            int parsed;
            bool anydata = false;

            if (_roomCnt1Idx >= 0)
            {
                field = row[_roomCnt1Idx].Trim();
                if (field.Length > 0)
                {
                    if (!int.TryParse(field, out parsed))
                        throw new InvalidDataException("Room count (Rms) value not understood.");
                    else
                    { result += parsed; anydata = true; }
                }
            }
            if (_roomCnt2Idx >= 0)
            {
                field = row[_roomCnt2Idx].Trim();
                if (field.Length > 0)
                {
                    if (!int.TryParse(field, out parsed))
                        throw new InvalidDataException("Room count (Rooms_plus) value not understood.");
                    else
                    { result += parsed; anydata = true; }
                }
            }

            if (result > 15) warnings.Append("Room count is not valid (>15)");

            return anydata ? result : -1;
        }

        private int getRoomCountAlt(string[] row, StringBuilder warnings)
        {
            int result = 0;
            string field;
            bool anydata = false;

            foreach (int idx in _roomTypeIdx)
            {
                if (idx >= 0)
                {
                    field = row[idx].Trim();
                    if (field.Length > 0) { result++; anydata = true; }
                }
            }

            if (result > 15) warnings.Append("Room count is not valid (>15)");

            return anydata ? result : -1;
        }

        private int getBathroomCountAlt(string[] row, StringBuilder warnings)
        {
            int result = 0;
            string field;
            bool anydata = false;

            foreach (int idx in _washroomTypeIdx)
            {
                if (idx >= 0)
                {
                    field = row[idx].Trim();
                    if (field.Length > 0) { result++; anydata = true; }
                }
            }

            if (result > 15) warnings.Append("Closet count is not valid (>15)");

            return anydata ? result : -1;
        }

        private int getBedroomCount(string[] row, StringBuilder warnings)
        {
            int result = 0;
            string field;
            int parsed;
            bool anydata = false;

            if (_bedroomCnt1Idx >= 0)
            {
                field = row[_bedroomCnt1Idx].Trim();
                if (field.Length > 0)
                {
                    if (!int.TryParse(field, out parsed))
                        throw new InvalidDataException("Bedroom count (Br) value not understood.");
                    else
                    { result += parsed; anydata = true; }
                }
            }
            if (_bedroomCnt2Idx >= 0)
            {
                field = row[_bedroomCnt2Idx].Trim();
                if (field.Length > 0)
                {
                    if (!int.TryParse(field, out parsed))
                        throw new InvalidDataException("Bedroom count (Br_plus) value not understood.");
                    else
                    { result += parsed; anydata = true; }
                }
            }

            if (result > 15) warnings.Append("Bedroom count is not valid (>15)");

            return anydata ? result : -1;
        }

        private int getBathroomCount(string[] row, StringBuilder warnings)
        {
            int result = 0;
            string field;
            int parsed;
            bool anydata = false;

            if (_bathroomCntIdx >= 0)
            {
                field = row[_bathroomCntIdx].Trim();
                if (field.Length > 0)
                {
                    if (!int.TryParse(field, out parsed))
                        throw new InvalidDataException("Bathroom count (Bath_tot) value not understood.");
                    else
                    { result += parsed; anydata = true; }
                }
            }

            if (result > 12) warnings.Append("Bathroom co unt is not valid (>12)");

            return anydata ? result : -1;
        }
    }
}