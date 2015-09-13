using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Vre.Server
{
    public class ModelImportSettings
    {
		public enum Mode { Site, Building, Structure }
		public enum LengthUnit { Feet, Meters }
		public enum AreaUnit { SqFeet, SqMeters }

        private Dictionary<string, string> _settings = new Dictionary<string, string>();
		private string _settingsFileName;

        public ModelImportSettings(string settingsFileName)
        {
            int lineNum = 0;
			_settingsFileName = settingsFileName;
            BasePath = Path.GetDirectoryName(_settingsFileName);
			if (File.Exists(_settingsFileName))
			{
				using (StreamReader rdr = File.OpenText(_settingsFileName))
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
        }

		public void Save()
		{
			using (var file = File.Create(_settingsFileName))
			{
				using (var wrt = new StreamWriter(file, Encoding.UTF8))
				{
					wrt.WriteLine("; 3D Condo Explorer model import settings");
					wrt.WriteLine("; MODIFY WITH CARE: This file is overwritten with ModelPackageTester import tool; comments etc. may be lost.");
					switch (ImportMode)
					{
						case Mode.Site:
							wrt.WriteLine("; {0} - complete site", SiteName);
							break;

						case Mode.Building:
							wrt.WriteLine("; {0} - single building", BuildingToImportName);
							break;

						case Mode.Structure:
							wrt.WriteLine("; {0} - single structure", StructureName);
							break;
					}
					wrt.WriteLine();

					foreach (var kvp in _settings)
						wrt.WriteLine("{0} = {1}", kvp.Key, kvp.Value);
				}
			}
		}

        private string getSetting(string key, string defaultValue)
        {
            string result;
            if (_settings.TryGetValue(key, out result)) return result;
            return defaultValue;
        }

		private void putSetting(string key, string value)
		{
			if (!string.IsNullOrWhiteSpace(value)) _settings[key] = value;
			else _settings.Remove(key);
		}

        public string BasePath { get; private set; }

		public Mode ImportMode
		{
			get
			{
				var test = getSetting("ImportMode", string.Empty).ToUpperInvariant();
				if (test.Equals("SITE")) return Mode.Site;
				else if (test.Equals("BUILDING")) return Mode.Building;
				else if (test.Equals("STRUCTURE")) return Mode.Structure;
				else if (BuildingToImportName != null) return Mode.Building;
				else return Mode.Site;
			}
			set { putSetting("ImportMode", value.ToString()); }
		}

		public LengthUnit HeightUnit
		{
			get
			{
				var test = getSetting("HeightUnit", string.Empty).ToUpperInvariant();
				if (test.Equals("METERS")) return ModelImportSettings.LengthUnit.Meters;
				else return ModelImportSettings.LengthUnit.Feet;
			}
			set { putSetting("HeightUnit", value.ToString()); }
		}

		public AreaUnit FloorAreaUnit
		{
			get
			{
				var test = getSetting("FloorAreaUnit", string.Empty).ToUpperInvariant();
				if (test.Equals("SQMETERS")) return ModelImportSettings.AreaUnit.SqMeters;
				else return ModelImportSettings.AreaUnit.SqFeet;
			}
			set { putSetting("FloorAreaUnit", value.ToString()); }
		}

		public string Currency
		{
			get { return getSetting("Currency", "CAD"); }
			set { putSetting("Currency", value); }
		}

		public string StructureName
		{
			get { return getSetting("StructureName", null); }
			set { putSetting("StructureName", value); }
		}
		
		public string ModelFileName 
		{ 
			get { return getSetting("ModelFileName", "wires.kmz"); }
			set { putSetting("ModelFileName", value); }
		}
        public string SuiteTypeInfoFileName 
		{ 
			get { return getSetting("SuiteTypeInfoFileName", "suitetypes.csv"); }
			set { putSetting("SuiteTypeInfoFileName", value); }
		}
        public string FloorplanDirectoryName 
		{ 
			get { return getSetting("FloorplanDirectoryName", "floorplans"); }
			set { putSetting("FloorplanDirectoryName", value); }
		}

        public string EstateDeveloperName 
		{ 
			get { return getSetting("EstateDeveloperName", null); }
			set { putSetting("EstateDeveloperName", value); }
		}
        public string SiteName 
		{ 
			get { return getSetting("SiteName", null); }
			set { putSetting("SiteName", value); }
		}

        public string DisplayModelFileName 
		{
			get { return getSetting("DisplayModelFileName", null); }
			set { putSetting("DisplayModelFileName", value); }
		}
        public string OverlayModelFileName 
		{ 
			get { return getSetting("OverlayModelFileName", null); }
			set { putSetting("OverlayModelFileName", value); }
		}
        public string PoiModelFileName 
		{ 
			get { return getSetting("PoiModelFileName", null); }
			set { putSetting("PoiModelFileName", value); }
		}

        public string BubbleWebTemplateFileName 
		{ 
			get { return getSetting("BubbleWebTemplateFileName", null); }
			set { putSetting("BubbleWebTemplateFileName", value); }
		}
        public string BubbleKioskTemplateFileName 
		{ 
			get { return getSetting("BubbleKioskTemplateFileName", null); }
			set { putSetting("BubbleKioskTemplateFileName", value); }
		}

        public string NewBuildingStatusName 
		{ 
			get { return getSetting("NewBuildingStatus", "Built"); }
			set { putSetting("NewBuildingStatus", value); }
		}
        public string NewSuiteStatusName 
		{ 
			get { return getSetting("NewSuiteStatus", "Sold"); }
			set { putSetting("NewSuiteStatus", value); }
		}

        public string BuildingToImportName 
		{ 
			get { return getSetting("BuildingToImportName", null); }
			set { putSetting("BuildingToImportName", value); }
		}
        public string BuildingStreetAddress 
		{ 
			get { return getSetting("BuildingStreetAddress", null); }
			set { putSetting("BuildingStreetAddress", value); }
		}
        public string BuildingMunicipality 
		{ 
			get { return getSetting("BuildingMunicipality", null); }
			set { putSetting("BuildingMunicipality", value); }
		}
        public string BuildingStateProvince 
		{ 
			get { return getSetting("BuildingStateProvince", null); }
			set { putSetting("BuildingStateProvince", value); }
		}
        public string BuildingPostalCode 
		{ 
			get { return getSetting("BuildingPostalCode", null); }
			set { putSetting("BuildingPostalCode", value); }
		}
        public string BuildingCountry 
		{ 
			get { return getSetting("BuildingCountry", null); }
			set { putSetting("BuildingCountry", value); }
		}

		//public string BuildingLocalizedName
		//{
		//    get { return getSetting("BuildingLocalizedName", null); }
		//    set { putSetting("BuildingLocalizedName", value); }
		//}
		public string StructureLocalizedName
		{
			get { return getSetting("StructureLocalizedName", null); }
			set { putSetting("StructureLocalizedName", value); }
		}
		//public string SiteLocalizedName
		//{
		//    get { return getSetting("SiteLocalizedName", null); }
		//    set { putSetting("SiteLocalizedName", value); }
		//}
	}
}