using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.Command;
using Vre.Server.Dao;
using Vre.Server.Mls;
using Vre.Server.RemoteService;
using System.IO;
using System.Linq;

namespace Vre.Server.Task
{
    internal class GenerateImportSettings : BaseTask
    {
		public override string Name { get { return "GenerateImportSettings"; } }

		protected static readonly char[] _invalidNameChars = Path.GetInvalidFileNameChars();

        public override void Execute(Parameters param)
        {
			var rootDir = param.GetOption("rootdir");
			if (null == rootDir) rootDir = @"C:\Users\vps3512Admin\Desktop\Models ready to Import";
			if (!Directory.Exists(rootDir)) throw new ArgumentException("Path " + rootDir + " does not exist.");

			StringBuilder report = new StringBuilder();
			int gcnt = 0, ncnt = 0;

			using (var session = ClientSession.MakeSystemSession())
			{
				DatabaseSettingsDao.VerifyDatabase();
				session.Resume();
				using (var dao = new BuildingDao(session.DbSession))
				{
					var buildings = dao.GetAll();
					foreach (var dir in Directory.GetDirectories(rootDir))
					{
						var files = Directory.GetFiles(dir, "*.import.txt");
						if (1 == files.Length)
						{
							var mis = new ModelImportSettings(files[0]);
							var candidates = buildings.Where(b => 
								b.Name.Equals(mis.BuildingToImportName, StringComparison.InvariantCultureIgnoreCase)
								&& b.ConstructionSite.Name.Equals(mis.SiteName, StringComparison.InvariantCultureIgnoreCase)
								&& b.ConstructionSite.Developer.Name.Equals(mis.EstateDeveloperName, StringComparison.InvariantCultureIgnoreCase));
							if (1 == candidates.Count())
							{
								var b = candidates.First();
								report.AppendFormat("Found existing import settings file for '{0}': {1}\r\n",
									b, files[0]);
								buildings.Remove(b);
							}
							else if (candidates.Count() > 1)
							{
								report.AppendFormat("ERROR: import settings file {0} matches multiple buildings:\r\n",
									files[0]);
								foreach (var b in candidates)
									report.AppendFormat("- '{0}'\r\n", b);
							}
						}
						else if (files.Length > 1)
						{
							report.AppendFormat("ERROR: directory {0} has multiple import settings files; not processed.\r\n",
								dir);
						}
						else
						{
							IEnumerable<Building> candidates;

							var id = lookupImportId(dir);
							if (id > 0)
							{
								candidates = buildings.Where(b => b.AutoID == id);
								if (1 == candidates.Count())
								{
									var b = candidates.First();
									report.AppendFormat("{0} guessed as '{1}' -> default.import.txt\r\n", dir, b);
									generateImportSettings(b, Path.Combine(dir, "default.import.txt"), dir);
									buildings.Remove(b);
									gcnt++;
									continue;
								}
							}

							var name = dir;
							if (name.EndsWith(" - Imported", StringComparison.InvariantCultureIgnoreCase))
								name = name.Substring(0, name.Length - 11);

							name = Path.GetFileNameWithoutExtension(name).Trim();
							candidates = buildings.Where(b => 
								b.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
							|| (b.AddressLine1 != null && b.AddressLine1.Equals(name, StringComparison.InvariantCultureIgnoreCase)));

							if (1 == candidates.Count())
							{
								var b = candidates.First();
								report.AppendFormat("{0} guessed as '{1}' -> default.import.txt\r\n", dir, b);
								generateImportSettings(b, Path.Combine(dir, "default.import.txt"), dir);
								buildings.Remove(b);
								gcnt++;
								continue;
							}

							bool found = false;
							foreach (var file in Directory.GetFiles(dir, "*.*"))
							{
								name = Path.GetFileNameWithoutExtension(file).Trim();
		
								candidates = buildings.Where(b =>
									b.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
								|| (b.AddressLine1 != null && b.AddressLine1.Equals(name, StringComparison.InvariantCultureIgnoreCase)));

								if (1 == candidates.Count())
								{
									var b = candidates.First();
									report.AppendFormat("{0} guessed as '{1}' -> default.import.txt\r\n", dir, b);
									generateImportSettings(b, Path.Combine(dir, "default.import.txt"), dir);
									buildings.Remove(b);
									gcnt++;
									found = true;
									break;
								}
							}

							if (!found)
								report.AppendFormat("WARNING: {0} is not guessed.\r\n", dir);
						}
					}
					foreach (var b in buildings)  // process remainings
					{
						var fn = string.Format("{0}-{1}.import.txt", b.AutoID, sanitizeFileNameElement(b.Name));
						var misf = Path.Combine(rootDir, fn);
						if (File.Exists(misf)) continue;
						report.AppendFormat("Not found '{0}' -> {1}\r\n", b, fn);
#if !DEBUG
						generateImportSettings(b, misf, null);
						ncnt++;
#endif
					}
				}
			}
			report.AppendFormat("Total guessed {0}, defaulted {1}.", gcnt, ncnt);

			ServiceInstances.Logger.Info(report.ToString());
        }

		protected static int lookupImportId(string baseDir)
		{
			int result = -1;
			var files = Directory.GetFiles(baseDir, "*.import.log.txt");
			if (1 == files.Length)
			{
				using (var rd = File.OpenText(files[0]))
				{
					while (!rd.EndOfStream)
					{
						var line = rd.ReadLine();
						var pos = line.IndexOf("building ID=");
						if (pos >= 0)
						{
							var pos2 = line.IndexOf(',', pos);
							if (pos2 > 0)
							{
								pos += 12;  // length of "building ID="
								if (!int.TryParse(line.Substring(pos, pos2 - pos), out result))
									result = -1;
							}
						}
					}
				}
			}
			return result;
		}

		protected static string sanitizeFileNameElement(string element)
		{
			int idx;
			do
			{
				idx = element.IndexOfAny(_invalidNameChars);
				if (idx >= 0) element = element.Remove(idx, 1);
			}
			while (idx >= 0);
			return element;
		}

		private static void generateImportSettings(Building b, string filePath, string baseDir)
		{
			ModelImportSettings mis = new ModelImportSettings(filePath);

			mis.ImportMode = ModelImportSettings.Mode.Building;
			mis.BuildingCountry = b.Country ?? "Canada";
			mis.BuildingStateProvince = b.StateProvince ?? "ON";
			mis.BuildingPostalCode = b.PostalCode;
			mis.BuildingMunicipality = b.City;
			if (!string.IsNullOrWhiteSpace(b.AddressLine2))
				mis.BuildingStreetAddress = b.AddressLine1 + " " + b.AddressLine2;
			else
				mis.BuildingStreetAddress = b.AddressLine1;
			mis.NewBuildingStatusName = "Built";
			mis.NewSuiteStatusName = "Sold";
			mis.BuildingToImportName = b.Name;
			mis.EstateDeveloperName = b.ConstructionSite.Developer.Name;
			mis.SiteName = b.ConstructionSite.Name;

			if (baseDir != null)
			{
				foreach (var file in Directory.GetFiles(baseDir))
				{
					var ext = Path.GetExtension(file).ToLowerInvariant().Substring(1);
					if (ext.Equals("kmz"))
					{
						mis.ModelFileName = Path.GetFileName(file);
					}
					else if (ext.Equals("kml"))
					{
						if (Path.GetFileNameWithoutExtension(file).Equals("wireframe-test-output", StringComparison.InvariantCultureIgnoreCase))
							continue;

						mis.PoiModelFileName = Path.GetFileName(file);
						//mis.BubbleKioskTemplateFileName;
						//mis.BubbleWebTemplateFileName;
						//mis.DisplayModelFileName;
						//mis.OverlayModelFileName;
					}
					else if (ext.Equals("csv"))
					{
						mis.SuiteTypeInfoFileName = Path.GetFileName(file);
					}
				}
				mis.FloorplanDirectoryName = guessFloorPlanPath(baseDir);
			}

			mis.Save();
		}

		private static string guessFloorPlanPath(string baseDir)
		{
			string test;

			test = Path.Combine(baseDir, "SuitesWeb\\images");
			if (Directory.Exists(test))
			{
				return "SuitesWeb\\images";
			}
			else
			{
				test = Path.Combine(baseDir, "SuitesWeb");
				if (Directory.Exists(test))
				{
					return "SuitesWeb";
				}
				else
				{
					test = Path.Combine(baseDir, "Site");
					if (Directory.Exists(test))
					{
						return "Site";
					}
					else
					{
						return ".";
					}
				}
			}
		}
    }
}