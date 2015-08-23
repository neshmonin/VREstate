using System;
using System.Collections.Generic;
using System.IO;

namespace Vre.Server.Util
{
	class LocalizationXrefFile
	{
		private readonly string _fileName;

		private bool _modified;

		private readonly Dictionary<string, string> _site = new Dictionary<string, string>();
		private readonly Dictionary<string, string> _building = new Dictionary<string, string>();
		private readonly Dictionary<string, string> _suiteType = new Dictionary<string, string>();
		private readonly Dictionary<string, string> _floor = new Dictionary<string, string>();
		private readonly Dictionary<string, string> _suite = new Dictionary<string, string>();
		private readonly Dictionary<string, string> _suiteLevel = new Dictionary<string, string>();

		public LocalizationXrefFile(string fileName)
		{
			_fileName = fileName;
			_modified = false;
		}

		public void Read()
		{
			_modified = false;
			_site.Clear();
			_building.Clear();
			_suiteType.Clear();
			_floor.Clear();
			_suite.Clear();
			_suiteLevel.Clear();

			using (var file = File.Open(_fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
			{
				using (var rdr = new StreamReader(file))
				{
					int lineNum = 0;
					Dictionary<string, string> currentDictionary = null;
					string line;
					while ((line = rdr.ReadLine()) != null)
					{
						lineNum++;
						if (string.IsNullOrWhiteSpace(line)) continue;

						line = line.Trim();

						if (line.StartsWith(";")) continue;  // comment

						if (line.StartsWith("["))  // section
						{
							var section = line.Substring(1, line.Length - 2).Trim();

							if (section.Equals("Sites")) currentDictionary = _site;
							else if (section.Equals("Buildings")) currentDictionary = _building;
							else if (section.Equals("SuiteTypes")) currentDictionary = _suiteType;
							else if (section.Equals("Floors")) currentDictionary = _floor;
							else if (section.Equals("Suites")) currentDictionary = _suite;
							else if (section.Equals("SuiteLevels")) currentDictionary = _suiteLevel;
							else throw new ApplicationException("Line " + lineNum + " contains unknown section name.");

							continue;
						}

						string[] parts = line.Split('=');
						if (parts.Length != 2)
							throw new ApplicationException("Line " + lineNum + " format is not valid.");

						if (null == currentDictionary)
							throw new ApplicationException("Line " + lineNum + " contains a key-value without a section defined.");

						string key = parts[0].Trim();
						if (currentDictionary.ContainsKey(key))
							throw new ApplicationException("Line " + lineNum + " contains duplicate setting key.");

						currentDictionary.Add(key, parts[1].Trim());
					}					
				}
			}
		}

		public void Save()
		{
			if (!_modified) return;
			using (var wrt = File.CreateText(_fileName))
			{
				wrt.WriteLine("; 3D Condo Explorer model localization cross-reference");
				wrt.WriteLine("; MODIFY WITH CARE: This file may be overwritten with import tool; comments etc. may be lost.");

				writeSection(wrt, "Sites", _site);
				writeSection(wrt, "Buildings", _building);
				writeSection(wrt, "SuiteTypes", _suiteType);
				writeSection(wrt, "Floors", _floor);
				writeSection(wrt, "Suites", _suite);
				writeSection(wrt, "SuiteLevels", _suiteLevel);
			}
		}

		private static void writeSection(StreamWriter dest, string sectionName, Dictionary<string, string> data)
		{
			if (0 == data.Count) return;
			dest.WriteLine("[{0}]", sectionName);
			foreach (var key in data.Keys) dest.WriteLine("{0}={1}", key, data[key]);
		}

		private static string get(string key, 
			Dictionary<string, string> dict)
		{
			if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("name is empty");
			string result;
			if (dict.TryGetValue(key, out result)) return result;
			return key;
		}

		private static void write(string name, string localized,
			Dictionary<string, string> dict, ref bool modified)
		{
			if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name is empty");
			if (string.IsNullOrWhiteSpace(localized)) throw new ArgumentException("localized is empty");
			string test;
			if (dict.TryGetValue(name, out test))
			{
				if (!test.Equals(localized)) modified = true;
			}
			else
			{
				modified = true;
			}
			dict[name] = localized;
		}

		#region get/set methods
		public string GetSiteName(string name)
		{
			return get(name, _site);
		}
		public void WriteSiteName(string name, string localized)
		{
			write(name, localized, _site, ref _modified);
		}
		public string GetBuildingName(string name)
		{
			return get(name, _building);
		}
		public void WriteBuildingName(string name, string localized)
		{
			write(name, localized, _building, ref _modified);
		}
		public string GetSuiteTypeName(string name)
		{
			return get(name, _suiteType);
		}
		public void WriteSuiteTypeName(string name, string localized)
		{
			write(name, localized, _suiteType, ref _modified);
		}
		public string GetFloorName(string name)
		{
			return get(name, _floor);
		}
		public void WriteFloorName(string name, string localized)
		{
			write(name, localized, _floor, ref _modified);
		}
		public string GetSuiteName(string name)
		{
			return get(name, _suite);
		}
		public void WriteSuiteName(string name, string localized)
		{
			write(name, localized, _suite, ref _modified);
		}
		public string GetSuiteLevelName(string name)
		{
			return get(name, _suiteLevel);
		}
		public void WriteSuiteLevelName(string name, string localized)
		{
			write(name, localized, _suiteLevel, ref _modified);
		}
		#endregion
	}
}