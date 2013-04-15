using System;
using System.Collections.Generic;
using System.IO;

namespace Vre.Server
{
    public class ModelImportSettings
    {
        private Dictionary<string, string> _settings = new Dictionary<string, string>();

        public ModelImportSettings(string settingsFileName)
        {
            int lineNum = 0;
            BasePath = Path.GetDirectoryName(settingsFileName);
            using (StreamReader rdr = File.OpenText(settingsFileName))
            {
                while (!rdr.EndOfStream)
                {
                    lineNum++;
                    string line = rdr.ReadLine().Trim();

                    if (0 == line.Length) continue;  // empty
                    if (line.StartsWith(";")) continue;  // comment

                    string[] parts = line.Split('=');
                    if (parts.Length != 2)
                        throw new ApplicationException("Line " + lineNum + " format is not valid.");

                    string key = parts[0].Trim();
                    if (_settings.ContainsKey(key))
                        throw new ApplicationException("Line " + lineNum + " contains duplicate setting key.");

                    _settings.Add(key, parts[1].Trim());
                }
            }
        }

        private string getSetting(string key, string defaultValue)
        {
            string result;
            if (_settings.TryGetValue(key, out result)) return result;
            return defaultValue;
        }

        public string BasePath { get; private set; }

        public string ModelFileName { get { return getSetting("ModelFileName", "wires.kmz"); } }
        public string SuiteTypeInfoFileName { get { return getSetting("SuiteTypeInfoFileName", "suitetypes.csv"); } }
        public string FloorplanDirectoryName { get { return getSetting("FloorplanDirectoryName", "floorplans"); } }

        public string EstateDeveloperName { get { return getSetting("EstateDeveloperName", null); } }
        public string SiteName { get { return getSetting("SiteName", null); } }

        public string DisplayModelFileName { get { return getSetting("DisplayModelFileName", null); } }
        public string OverlayModelFileName { get { return getSetting("OverlayModelFileName", null); } }
        public string PoiModelFileName { get { return getSetting("PoiModelFileName", null); } }

        public string BubbleWebTemplateFileName { get { return getSetting("BubbleWebTemplateFileName", null); } }
        public string BubbleKioskTemplateFileName { get { return getSetting("BubbleKioskTemplateFileName", null); } }

        public string NewBuildingStatusName { get { return getSetting("NewBuildingStatus", "Built"); } }
        public string NewSuiteStatusName { get { return getSetting("NewSuiteStatus", "Sold"); } }

        public string BuildingToImportName { get { return getSetting("BuildingToImportName", null); } }
        public string BuildingStreetAddress { get { return getSetting("BuildingStreetAddress", null); } }
        public string BuildingMunicipality { get { return getSetting("BuildingMunicipality", null); } }
        public string BuildingStateProvince { get { return getSetting("BuildingStateProvince", null); } }
        public string BuildingPostalCode { get { return getSetting("BuildingPostalCode", null); } }
        public string BuildingCountry { get { return getSetting("BuildingCountry", null); } }
    }
}