using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Vre.Server.Model
{
    public class CsvSuiteTypeInfo
    {
        private int _typeNameIdx = -1;
        private int _levelsIdx = -1;
        private int _indoorAreaIdx = -1;
        private int _outdoorAreaIdx = -1;
        private int _bedroomsIdx = -1;
        private int _densIdx = -1;
        private int _otherRoomsIdx = -1;
        private int _showerBathIdx = -1;
        private int _noShowerBathIdx = -1;
        private int _balconiesIdx = -1;
        private int _terracesIdx = -1;
        private int _floorPlanNameIdx = -1;

        private Dictionary<string, string[]> _rawData;

        public CsvSuiteTypeInfo(string fileName, StringBuilder readWarnings)
        {
            if (null == fileName) return;

            _rawData = new Dictionary<string, string[]>();
            using (FileStream fs = File.OpenRead(fileName))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string line = sr.ReadLine();
                    string[] parts = CsvUtilities.Split(line);
                    int partCnt = parts.Length;
                    //if (10 != parts.Length) throw new ArgumentException("The suite type information file is not of correct format");
                    parseHeader(parts);

                    if (_typeNameIdx < 0) throw new ArgumentException("The suite type information file is not of correct format (0)");

                    int lineNum = 1;
                    do
                    {
                        line = sr.ReadLine();
                        lineNum++;

                        if (string.IsNullOrWhiteSpace(line)) continue;

                        parts = CsvUtilities.Split(line);
                        if (partCnt != parts.Length)
                        {
                            if (readWarnings != null)
                                readWarnings.AppendFormat("\r\nSTMD04: Suite type information file line {0} format is invalid (missing fields?)", lineNum);
                            else
                                throw new ArgumentException("The suite type information file is not of correct format (line " + lineNum + ") (1)");
                        }

                        if (_rawData.ContainsKey(parts[_typeNameIdx]))
                        {
                            if (readWarnings != null)
                                readWarnings.AppendFormat("\r\nSTMD05: Suite type information file contains duplicate type name (line {0}): {1}", lineNum, parts[_typeNameIdx]);
                            else
                                throw new ArgumentException(
                                    "Suite type information file contains duplicate type name (line " + lineNum + "): " + parts[_typeNameIdx]);
                        }

                        if (string.IsNullOrWhiteSpace(parts[_typeNameIdx]))
                        {
                            if (readWarnings != null)
                                readWarnings.AppendFormat("\r\nSTMD06: Suite type information file contains empty type name (line {0}).", lineNum);
                            else
                                throw new ArgumentException(
                                    "Suite type information file contains empty type name (line " + lineNum + "): " + parts[_typeNameIdx]);
                        }
                            
                        _rawData.Add(parts[_typeNameIdx], parts);

                        for (int ii = 0; ii < parts.Length; ii++)
                        {
                            if (string.IsNullOrWhiteSpace(parts[ii]))
                            {
                                if (readWarnings != null)
                                    readWarnings.AppendFormat("\r\nSTMD03: Warning: line {0} ({1}): item \'{2}\' cannot be empty.",
                                        lineNum, parts[_typeNameIdx], headerItemName(ii));
                            }
                            else if (parts[ii].Equals("?"))
                            {
                                parts[ii] = string.Empty;
                            }
                        }
                    }
                    while (!sr.EndOfStream);
                }
            }

            if (readWarnings != null)
            {
                if (_typeNameIdx < 0) readWarnings.Append("\r\nSTMD02: TYPENAME column in SuiteTypeInfo file is missing!");
                if (_levelsIdx < 0) readWarnings.Append("\r\nSTMD02: LEVELS column in SuiteTypeInfo file is missing!");
                if (_indoorAreaIdx < 0) readWarnings.Append("\r\nSTMD02: TOTALINDOORAREA column in SuiteTypeInfo file is missing!");
                if (_outdoorAreaIdx < 0) readWarnings.Append("\r\nSTMD02: TOTALOUTDOORAREA column in SuiteTypeInfo file is missing!");
                if (_bedroomsIdx < 0) readWarnings.Append("\r\nSTMD02: BEDROOMS column in SuiteTypeInfo file is missing!");
                if (_densIdx < 0) readWarnings.Append("\r\nSTMD02: DENS column in SuiteTypeInfo file is missing!");
                if (_otherRoomsIdx < 0) readWarnings.Append("\r\nSTMD02: OTHERROOMS column in SuiteTypeInfo file is missing!");
                if (_showerBathIdx < 0) readWarnings.Append("\r\nSTMD02: SHOWER/BATH column in SuiteTypeInfo file is missing!");
                if (_noShowerBathIdx < 0) readWarnings.Append("\r\nSTMD02: NOSHOWER/BATH column in SuiteTypeInfo file is missing!");
                if (_balconiesIdx < 0) readWarnings.Append("\r\nSTMD02: BALCONIES column in SuiteTypeInfo file is missing!");
                if (_terracesIdx < 0) readWarnings.Append("\r\nSTMD02: TERRACES column in SuiteTypeInfo file is missing!");
                if (_floorPlanNameIdx < 0) readWarnings.Append("\r\nSTMD02: FLOORPLANFILENAME column in SuiteTypeInfo file is missing!");
            }
        }

        public IEnumerable<string> TypeInfoList { get { return _rawData.Keys; } }

        private void parseHeader(string[] items)
        {
            // Possible header items:
            // TypeName,Levels,TotalIndoorArea,TotalOutdoorArea,Bedrooms,Dens,OtherRooms,Shower/Bath,NoShower/Bath,Balconies,Terrases,FloorplanFileName

            for (int idx = items.Length - 1; idx >= 0; idx--)
            {
                string item = items[idx].Trim().ToUpper();

                if (item.Equals("TYPENAME")) _typeNameIdx = idx;
                else if (item.Equals("LEVELS")) _levelsIdx = idx;
                else if (item.Equals("TOTALINDOORAREA")) _indoorAreaIdx = idx;
                else if (item.Equals("TOTALOUTDOORAREA")) _outdoorAreaIdx = idx;
                else if (item.Equals("BEDROOMS")) _bedroomsIdx = idx;
                else if (item.Equals("DENS")) _densIdx = idx;
                else if (item.Equals("OTHERROOMS")) _otherRoomsIdx = idx;
                else if (item.Equals("SHOWER/BATH")) _showerBathIdx = idx;
                else if (item.Equals("NOSHOWER/BATH")) _noShowerBathIdx = idx;
                else if (item.Equals("BALKONIES")) _balconiesIdx = idx;  // syntax error :)
                else if (item.Equals("BALCONIES")) _balconiesIdx = idx;
                else if (item.Equals("TERRASES")) _terracesIdx = idx;  // syntax error :)
                else if (item.Equals("TERRACES")) _terracesIdx = idx;
                else if (item.Equals("FLOORPLANFILENAME")) _floorPlanNameIdx = idx;
            }
        }

        private string headerItemName(int index)
        {
            if (index == _typeNameIdx) return "TypeName";
            else if (index == _levelsIdx) return "Levels";
            else if (index == _indoorAreaIdx) return "TotalIndoorArea";
            else if (index == _outdoorAreaIdx) return "TotalOutdoorArea";
            else if (index == _bedroomsIdx) return "Bedrooms";
            else if (index == _densIdx) return "Dens";
            else if (index == _otherRoomsIdx) return "OtherRooms";
            else if (index == _showerBathIdx) return "Shower/Bath";
            else if (index == _noShowerBathIdx) return "NoShower/Bath";
            else if (index == _balconiesIdx) return "Balconies";
            else if (index == _terracesIdx) return "Terraces";
            else if (index == _floorPlanNameIdx) return "FloorplanFileName";

            return "";
        }

        private string[] getPropArray(string suiteType)
        {
            if (null == _rawData) return null;
            string[] result;
            if (_rawData.TryGetValue(suiteType, out result)) return result;
            return null;
        }

        private string getProp(string suiteType, int pos, string defValue)
        {
            string result = defValue;
            string[] arr = getPropArray(suiteType);
            if (arr != null) result = arr[pos];
            return result;
        }

        private double getProp(string suiteType, int pos, double defValue)
        {
            double result;
            string[] arr = getPropArray(suiteType);
            if ((arr != null) && !string.IsNullOrWhiteSpace(arr[pos]) && double.TryParse(arr[pos], out result))
                return result;
            return defValue;
        }

        private int getProp(string suiteType, int pos, int defValue)
        {
            int result;
            string[] arr = getPropArray(suiteType);
            if ((arr != null) && !string.IsNullOrWhiteSpace(arr[pos]) && int.TryParse(arr[pos], out result))
                return result;
            return defValue;
        }

        private bool getProp(string suiteType, int pos, bool defValue)
        {
            bool result;
            string[] arr = getPropArray(suiteType);
            if ((arr != null) && !string.IsNullOrWhiteSpace(arr[pos]) && tryParse(arr[pos], out result))
                return result;
            return defValue;
        }

        private bool tryParse(string str, out bool result)
        {
            str = str.ToLower();

            if (str.Equals("yes") || str.Equals("true")
                || str.Equals("y") || str.Equals("1")) { result = true; return true; }
            else if (str.Equals("no") || str.Equals("false")
                || str.Equals("n") || str.Equals("0")) { result = false; return true; }
            else { result = false; return false; }
        }

        public bool HasType(string suiteType)
        {
            return (getPropArray(suiteType) != null);
        }

        public double GetIndoorFloorAreaSqFt(string suiteType)
        {
            if (_indoorAreaIdx < 0) return 0.0;
            return getProp(suiteType, _indoorAreaIdx, 0.0);
        }

        public double GetOutdoorFloorAreaSqFt(string suiteType)
        {
            if (_outdoorAreaIdx < 0) return 0.0;
            return getProp(suiteType, _outdoorAreaIdx, 0.0);
        }

        public int GetBedroomCount(string suiteType)
        {
            if (_bedroomsIdx < 0) return 0;
            return getProp(suiteType, _bedroomsIdx, 0);
        }

        public int GetDenCount(string suiteType)
        {
            if (_densIdx < 0) return 0;
            return getProp(suiteType, _densIdx, 0);
        }

        public int GetOtherRoomsCount(string suiteType)
        {
            if (_otherRoomsIdx < 0) return 0;
            return getProp(suiteType, _otherRoomsIdx, 0);
        }

        public int GetNoShowerBathroomCount(string suiteType)
        {
            if (_noShowerBathIdx < 0) return 0;
            return getProp(suiteType, _noShowerBathIdx, 0);
        }

        public int GetShowerBathroomCount(string suiteType)
        {
            if (_showerBathIdx < 0) return 0;
            return getProp(suiteType, _showerBathIdx, 0);
        }

        public int GetBalconyCount(string suiteType)
        {
            if (_balconiesIdx < 0) return 0;
            return getProp(suiteType, _balconiesIdx, 0);
        }

        public int GetTerraceCount(string suiteType)
        {
            if (_terracesIdx < 0) return 0;
            return getProp(suiteType, _terracesIdx, 0);
        }

        public int GetLevels(string suiteType)
        {
            if (_levelsIdx < 0) return 0;
            return getProp(suiteType, _levelsIdx, 0);
        }

        public string GetFloorPlanFileName(string suiteType)
        {
            if (_floorPlanNameIdx < 0) return null;
            return getProp(suiteType, _floorPlanNameIdx, string.Empty);
        }
    }
}