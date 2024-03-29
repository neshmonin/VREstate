﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Text;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;
using Vre.Server.FileStorage;
using Vre.Server.Mls;
using Vre.Server.Model;
using Vre.Server.RemoteService;
using Vre.Server.Util;

namespace Vre.Server.Command
{
	public class MiTest
	{
		public static void Test()
		{
			using (var _clientSession = ClientSession.MakeSystemSession())//_session))
			{
				ICollection<int> importedBuildingIds = new List<int>();

				_clientSession.Resume();
				DatabaseSettingsDao.VerifyDatabase();
				//_clientSession.DbSession.FlushMode = FlushMode.Always;
				using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_clientSession.DbSession))
				{
					EstateDeveloper developer;
					Site site = null;

					using (EstateDeveloperDao dao = new EstateDeveloperDao(_clientSession.DbSession))
						developer = dao.GetById("Resale");

					foreach (var s in developer.Sites)
						s.Name.Equals(s.AutoID);
				}
			}
		}
	}

    internal class ModelImport : ICommand
    {
        private CsvSuiteTypeInfo _extraSuiteInfo;
        private StringBuilder _log;
        private Dictionary<string, SuiteType> _typeCache;
        private ClientSession _clientSession;
        private string _importPath;
        private List<string> _filesSaved = new List<string>();
		private Currency _currency;
		private ValueWithUM.Unit _floorAreaUnit;
		private ValueWithUM.Unit _heightUnit;
		private LocalizationXrefFile _localizationXref;
        //private ISession _session;

        public string Name { get { return "importmodel"; } }

        public void Execute(Parameters param)
        {
            string infoModelFileName = param.GetOption("infomodel");
            string estateDeveloper = param.GetOption("ed");

            if (string.IsNullOrWhiteSpace(infoModelFileName)) throw new ArgumentException("Required parameter missing: infomodel");
            if (string.IsNullOrWhiteSpace(estateDeveloper)) throw new ArgumentException("Required parameter missing: ed");

            string extraSuiteInfoFileName = param.GetOption("sti");
            string siteName = param.GetOption("site");
            bool dryRun = CommandHandler.str2bool(param.GetOption("dryrun"), true);

			if (!ValueWithUM.TryParseUnit(param.GetOption("heightunit") ?? ValueWithUM.Unit.Feet.ToString(), out _heightUnit))
				throw new ArgumentException("Invalid heightunit provided");

			if (!ValueWithUM.TryParseUnit(param.GetOption("floorareaunit") ?? ValueWithUM.Unit.Feet.ToString(), out _floorAreaUnit))
				throw new ArgumentException("Invalid floorareaunit provided");

			if (!Currency.TryParse(param.GetOption("currency") ?? Vre.Server.BusinessLogic.Utilities.DefaultCurrency.Iso3LetterCode, out _currency))
				throw new ArgumentException("Unknown currency provided");

			var derivedFileName = Path.Combine(
					Path.GetDirectoryName(infoModelFileName),
					Path.GetFileNameWithoutExtension(infoModelFileName));

			_log = new StringBuilder();
            var logFileName = derivedFileName + ".import.log.txt";
			var localizationDictionaryFileName = derivedFileName + "-localized-xref.txt";

            _log.AppendLine("=========================================================");
            _log.AppendLine(Environment.CommandLine);
			_log.AppendFormat("Using the following localization cross-reference: {0}{1}", localizationDictionaryFileName, Environment.NewLine);
			_log.AppendLine("---------------------------------------------------------");

            Exception importError = null;
			try
			{
				DatabaseSettingsDao.VerifyDatabase();

				debugBreak();

				_localizationXref = new LocalizationXrefFile(localizationDictionaryFileName);
				_localizationXref.Read();

				doImport(estateDeveloper, siteName,
					infoModelFileName, extraSuiteInfoFileName, dryRun, param);
				//instance.generateSqlScript(estateDeveloperName, siteName, modelFileName, extraSuiteInfoFileName);

				_localizationXref.Save();
			}
			catch (Exception e)
			{
				importError = e;
				_log.AppendFormat("Error importing model: {0}", Utilities.ExplodeException(e));
			}
			finally
			{
				using (var logFile = File.Open(logFileName, FileMode.Append, FileAccess.Write, FileShare.Read))
				{
					using (var sw = new StreamWriter(logFile)) sw.Write(_log);
				}
			}

            if (importError != null) throw importError;
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private static void debugBreak()
        {
            System.Diagnostics.Debugger.Break();
        }

        private void generateSqlScript(string estateDeveloperName, string siteName,
            string modelFileName, string extraSuiteInfoFileName)
        {
            _extraSuiteInfo = new CsvSuiteTypeInfo(extraSuiteInfoFileName, null);

            StringBuilder readWarnings = new StringBuilder();
            Vre.Server.Model.Kmz.Kmz kmz = new Vre.Server.Model.Kmz.Kmz(modelFileName, readWarnings);

            using (FileStream file = File.Create(kmz.Model.Site.Name + ".sql"))
            {
                using (StreamWriter sw = new StreamWriter(file))
                {
                    sw.WriteLine("begin tran");
                    sw.WriteLine("declare @sid int");
                    sw.WriteLine("declare @buildingid int");
                    sw.WriteLine("declare @siteid int");
                    sw.WriteLine("set @siteid = -1");

                    sw.WriteLine("declare @timestamp datetime");
                    sw.WriteLine("set @timestamp = getdate()");

                    Dictionary<string, string> stypes = new Dictionary<string, string>();

                    foreach (Vre.Server.Model.Kmz.Building bldg in kmz.Model.Site.Buildings)
                    {
                        foreach (Vre.Server.Model.Kmz.Suite s in bldg.Suites)
                        {
                            if (!stypes.ContainsKey(s.ClassName))
                            {
                                string variable = "@st_" + s.ClassName.Replace('-', '_').Replace('.', '_');
                                sw.WriteLine("declare " + variable + " int");
                                sw.WriteLine("select " + variable + " = [autoid] from [suitetypes] where [siteid] = @siteid and [name] = '" + s.ClassName + "'");
                                sw.WriteLine("if " + variable + " is null begin");
                                sw.WriteLine("  insert into [suitetypes] ([created],[updated],[deleted],[name],[siteid],[floorarea],[bedroomcnt],[dencnt],[bathroomcnt],[balconycnt],[terracecnt]) values(@timestamp,@timestamp,0,'{0}',@siteid,'{1},2',{2},{3},{4},{5},{6})",
                                    s.ClassName, _extraSuiteInfo.GetIndoorFloorAreaSqFt(s.ClassName),
                                    _extraSuiteInfo.GetBedroomCount(s.ClassName),
                                    _extraSuiteInfo.GetDenCount(s.ClassName),
                                    _extraSuiteInfo.GetShowerBathroomCount(s.ClassName) + _extraSuiteInfo.GetNoShowerBathroomCount(s.ClassName),
                                    _extraSuiteInfo.GetBalconyCount(s.ClassName),
                                    _extraSuiteInfo.GetTerraceCount(s.ClassName));
                                sw.WriteLine("  set " + variable + " = @@identity");
                                sw.WriteLine("end");
                                stypes.Add(s.ClassName, variable);
                            }
                        }
                    }

                    foreach (Vre.Server.Model.Kmz.Building bldg in kmz.Model.Site.Buildings)
                    {
                        sw.WriteLine("set @buildingid = null");
                        sw.WriteLine("select @buildingid = [autoid] from [buildings] where [name] = '{0}' and siteid = @siteid", bldg.Name);
                        sw.WriteLine("if @buildingid is null begin");
                        sw.WriteLine("  insert into [buildings] ([created],[updated],[deleted],[name],[status],[siteid]) values(@timestamp,@timestamp,0,'{0}',0,@siteid)",
                            bldg.Name);
                        sw.WriteLine("  set @buildingid = @@identity");
                        sw.WriteLine("end");
                        foreach (Vre.Server.Model.Kmz.Suite s in bldg.Suites)
                        {
                            sw.WriteLine("set @sid = null");
                            sw.WriteLine("select @sid = [autoid] from [suites] where [buildingid] = @buildingid and [suitename] = '" + s.Name + "'");
                            sw.WriteLine("if @sid is null begin");
                            sw.WriteLine("  insert into [suites] ([created],[updated],[deleted],[buildingid],[physicallevelnumber],[floorname],[suitename],[status],[suitetypeid],[ceilingheight],[showpanoramicview]) values(@timestamp,@timestamp,0,@buildingid,{0},'{1}','{2}',0,{3},{4},{5})",
                                1, "1", s.Name, stypes[s.ClassName], s.CeilingHeightFt, s.ShowPanoramicView ? "1" : "0");
                            sw.WriteLine("end else begin");
                            sw.WriteLine("  update [suites] set [updated] = @timestamp, [suitetypeid] = {0}, [ceilingheight] = {1}, [showpanoramicview] = {2} where [autoid] = @sid",
                                stypes[s.ClassName], s.CeilingHeightFt, s.ShowPanoramicView ? "1" : "0");
                            sw.WriteLine("end");
                        }
                    }
                    sw.WriteLine("commit tran");
                }
            }

        }

        private void doImport(string estateDeveloperName, string siteName, 
            string infoModelFileName, string extraSuiteInfoFileName, 
            bool dryRun, Parameters extras)
        {
            _extraSuiteInfo = new CsvSuiteTypeInfo(extraSuiteInfoFileName, null);

            if (!string.IsNullOrWhiteSpace(extraSuiteInfoFileName) && File.Exists(extraSuiteInfoFileName))
                _importPath = Path.GetDirectoryName(extraSuiteInfoFileName);
            else
                _importPath = Path.GetDirectoryName(infoModelFileName);

            //_session = NHibernateHelper.GetSession();
            using (_clientSession = ClientSession.MakeSystemSession())//_session))
            {
				ICollection<int> importedBuildingIds = new List<int>();

                _clientSession.Resume();
				DatabaseSettingsDao.VerifyDatabase();
                //_clientSession.DbSession.FlushMode = FlushMode.Always;
                using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_clientSession.DbSession))
                {
                    EstateDeveloper developer;
                    Site site = null;
                    _typeCache = new Dictionary<string, SuiteType>();

                    using (EstateDeveloperDao dao = new EstateDeveloperDao(_clientSession.DbSession))
                        developer = dao.GetById(estateDeveloperName);

                    if (null == developer) throw new ObjectNotFoundException(estateDeveloperName, "EstateDeveloper");
                    _log.AppendFormat("Using estate developer ID={0}, Name={1}\r\n", developer.AutoID, developer.Name);

                    bool siteCreated = false;

                    if (siteName != null)
                    {
                        using (SiteDao dao = new SiteDao(_clientSession.DbSession))
                            site = dao.GetById(developer, siteName);

                        if (null == site)
                        {
                            site = new Site(developer, siteName);
                            _clientSession.DbSession.Save(site);
                            siteCreated = true;
                            _log.AppendFormat("Created new site provided by command-line ID={0}, Name={1}\r\n", site.AutoID, site.Name);
                        }
                        else
                        {
                            _log.AppendFormat("Using command-line override site ID={0}, Name={1}\r\n", site.AutoID, site.Name);
                        }
                    }

                    StringBuilder readWarnings = new StringBuilder();
                    Vre.Server.Model.Kmz.Kmz kmz = new Vre.Server.Model.Kmz.Kmz(infoModelFileName, readWarnings);

                    if (null == site)
                    {
                        using (SiteDao dao = new SiteDao(_clientSession.DbSession))
                            site = dao.GetById(developer, kmz.Model.Site.Name);

                        if (null == site)
                        {
                            site = new Site(developer, kmz.Model.Site.Name);
                            _clientSession.DbSession.Save(site);
                            siteCreated = true;
                            _log.AppendFormat("Created new site provided by model ID={0}, Name={1}\r\n", site.AutoID, site.Name);
                        }
                        else
                        {
                            _log.AppendFormat("Using site provided by model ID={0}, Name={1}\r\n", site.AutoID, site.Name);
                        }
                    }

					importSite(kmz.Model.Site, site, siteCreated, infoModelFileName, extras, ref importedBuildingIds);

                    if (dryRun)
                    {
                        tran.Rollback();
                        foreach (string file in _filesSaved)
                        {
                            try { ServiceInstances.InternalFileStorageManager.RemoveFile(file); }
                            catch (FileNotFoundException) { ServiceInstances.FileStorageManager.RemoveFile(file); }
                        }
                        _log.Append("DRY RUN MODE: all changes rolled back.\r\n");
                    }
                    else
                    {
                        //_clientSession.DbSession.Flush();
                        tran.Commit();
                        _log.Append("All changes comitted to database.\r\n");
                    }
                }  // transaction

				if (!dryRun && CommandHandler.str2bool(extras.GetOption("mlsimport"), true))
				{
					foreach (var id in importedBuildingIds)
						TryImportExistingListings(_clientSession, id, _log);
				}
            }  // client session
        }

        private void importSite(Vre.Server.Model.Kmz.ConstructionSite modelSite, Site dbSite, bool isCreated,            
            string infoModelFileName, Parameters extras,
			ref ICollection<int> importedBuildingIds)
        {
            string displayModelFileName = extras.GetOption("displaymodel");
            string overlayModelFileName = extras.GetOption("overlaymodel");
            string poiModelFileName = extras.GetOption("poimodel");
            string bubbleWebTemplateFileName = extras.GetOption("bubblewebtemplate");
            string bubbleKioskTemplateFileName = extras.GetOption("bubblekiosktemplate");
            bool isSiteModel = !CommandHandler.str2bool(extras.GetOption("asbuilding"), false);
            string singleBuildingName = extras.GetOption("building");

            Building.BuildingStatus newBuildingStatus;
            if (!Enum.TryParse<Building.BuildingStatus>(extras.GetOption("buildingStatus"), out newBuildingStatus))
                newBuildingStatus = Building.BuildingStatus.Sold;

            List<string> missingBuildings = new List<string>(dbSite.Buildings.Count);
            foreach (Building b in dbSite.Buildings) missingBuildings.Add(b.Name);

            foreach (Vre.Server.Model.Kmz.Building mb in modelSite.Buildings)
            {
                // single building import
                if ((singleBuildingName != null) && !singleBuildingName.Equals(mb.Name, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                bool created = false;
                Building dbb = null;
                foreach (Building b in dbSite.Buildings)
                    if (b.Name.Equals(mb.Name) && !b.Deleted) { dbb = b; break; }

                if (null == dbb)
                {
                    foreach (Building b in dbSite.Buildings)
                        if (b.Name.Equals(mb.Name)) { dbb = b; break; }
                }

                if (null == dbb)
                {
                    dbb = new Building(dbSite, mb.Name);
                    dbb.Status = newBuildingStatus;
                    _clientSession.DbSession.Save(dbb);
                    created = true;
                    _log.AppendFormat("Created new building ID={0}, Name={1}\r\n", dbb.AutoID, dbb.Name);
                }
                else
                {
                    if (dbb.Deleted)
                    {
                        dbb.Undelete();
                        using (BuildingDao dao = new BuildingDao(_clientSession.DbSession))
                            if (!dao.SafeUpdate(dbb)) throw new StaleObjectStateException("Building", dbb.AutoID);

                        _log.AppendFormat("Recovered deleted building ID={0}, Name={1}\r\n", dbb.AutoID, dbb.Name);
                    }
                    else
                    {
                        _log.AppendFormat("Updating existing building ID={0}, Name={1}\r\n", dbb.AutoID, dbb.Name);
                    }
                    missingBuildings.Remove(dbb.Name);
                }

                importBuilding(modelSite, mb, dbb, created,
                    isSiteModel ? null : infoModelFileName,
                    isSiteModel ? null : displayModelFileName,
                    isSiteModel ? null : overlayModelFileName,
                    isSiteModel ? null : poiModelFileName,
                    isSiteModel ? null : bubbleWebTemplateFileName,
                    isSiteModel ? null : bubbleKioskTemplateFileName,
                    extras, ref importedBuildingIds);
            }

			if (isSiteModel)
			{
				// hide missing buildings
				//
				foreach (string bn in missingBuildings)
				{
					Building dbb = null;
					foreach (Building b in dbSite.Buildings)
						if (b.Name.Equals(bn) && !b.Deleted) { dbb = b; break; }

					if (dbb != null)
					{
						using (BuildingDao dao = new BuildingDao(_clientSession.DbSession))
							if (!dao.SafeDelete(dbb)) throw new StaleObjectStateException("Building", dbb.AutoID);
						_log.AppendFormat("Removed building missing in model ID={0}, Name={1}\r\n", dbb.AutoID, dbb.Name);
					}
				}

				// find and hide unused suite types
				//
				List<string> missingSuiteTypes = new List<string>(dbSite.SuiteTypes.Count);
				foreach (SuiteType st in dbSite.SuiteTypes) missingSuiteTypes.Add(st.Name);
				foreach (string stn in _typeCache.Keys) missingSuiteTypes.Remove(stn);
				foreach (string stn in missingSuiteTypes)
				{
					SuiteType dbst = null;
					foreach (SuiteType st in dbSite.SuiteTypes)
						if (st.Name.Equals(stn) && !st.Deleted) { dbst = st; break; }

					if (dbst != null)
					{
						using (SuiteTypeDao dao = new SuiteTypeDao(_clientSession.DbSession))
							if (!dao.SafeDelete(dbst)) throw new StaleObjectStateException("SuiteType", dbst.AutoID);
						_log.AppendFormat("Removed suite type missing in model ID={0}, Name={1}\r\n", dbst.AutoID, dbst.Name);
					}
				}

				// Update wireframe file
				//
				dbSite.WireframeLocation = storeModelFile(ServiceInstances.InternalFileStorageManager,
					dbSite, dbSite.WireframeLocation, infoModelFileName, "s", "wireframes");

				if (displayModelFileName != null)
					dbSite.DisplayModelUrl = storeModelFile(dbSite, dbSite.DisplayModelUrl, displayModelFileName, "s");

				if (overlayModelFileName != null)
					dbSite.OverlayModelUrl = storeModelFile(dbSite, dbSite.OverlayModelUrl, overlayModelFileName, "so");

				if (poiModelFileName != null)
					dbSite.PoiModelUrl = storeModelFile(dbSite, dbSite.PoiModelUrl, poiModelFileName, "sp");

				if (bubbleWebTemplateFileName != null)
					dbSite.BubbleWebTemplateUrl = storeModelFile(dbSite, dbSite.BubbleWebTemplateUrl, bubbleWebTemplateFileName, "sbwt");

				if (bubbleKioskTemplateFileName != null)
					dbSite.BubbleKioskTemplateUrl = storeModelFile(dbSite, dbSite.BubbleKioskTemplateUrl, bubbleKioskTemplateFileName, "sbkt");

				// Geoinformation import/update
				dbSite.Location = modelSite.LocationCart.AsGeoPoint();
			}
			else  // single building model
			{
				_clientSession.DbSession.Refresh(dbSite);  // update to reflect building(s) imported

				// promote POI to site level if available
				if (poiModelFileName != null)
					dbSite.PoiModelUrl = storeModelFile(dbSite, dbSite.PoiModelUrl, poiModelFileName, "sp");

				// Calculate and set geoinformation
				//
				double mLon = 0.0, mLat = 0.0, mAlt = 0.0;
				int buildingCnt = 0;
				foreach (var b in dbSite.Buildings)
				{
					var vp = b.Location;
					mLon += vp.Longitude;
					mLat += vp.Latitude;
					mAlt += vp.Altitude;
					buildingCnt++;
				}
				dbSite.Location = new GeoPoint(
					mLon / (double)buildingCnt,
					mLat / (double)buildingCnt,
					mAlt / (double)buildingCnt);
			}

			// set localized name: if provided - use it; else write it to XREF
			var locName = conditionString(_localizationXref.GetSiteName(dbSite.Name), 256);
			if (string.IsNullOrEmpty(locName)) _localizationXref.WriteSiteName(dbSite.Name, dbSite.LocalizedName);
			else dbSite.LocalizedName = locName;

			dbSite.MarkUpdated();
			_clientSession.DbSession.Update(dbSite);
        }

        private void importBuilding(Vre.Server.Model.Kmz.ConstructionSite modelSite,
			Vre.Server.Model.Kmz.Building modelBuilding, Building dbBuilding, bool isCreated,
            string infoModelFileName, string displayModelFileName, string overlayModelFileName, string poiModelFileName,
            string bubbleWebTemplateFileName, string bubbleKioskTemplateFileName,
            Parameters extras,
			ref ICollection<int> importedBuildingIds)
        {
            List<string> missingSuites = new List<string>(dbBuilding.Suites.Count);

            Suite.SalesStatus newSuiteStatus;
            if (!Enum.TryParse<Suite.SalesStatus>(extras.GetOption("suiteStatus"), out newSuiteStatus))
                newSuiteStatus = Suite.SalesStatus.Sold;

            foreach (Suite s in dbBuilding.Suites) missingSuites.Add(s.SuiteName);

            foreach (Vre.Server.Model.Kmz.Suite ms in modelBuilding.Suites)
            {
                bool created = false;
                Suite dbs = null;
                string modelSuiteName = Utilities.NormalizeSuiteNumber(ms.Name);
                foreach (Suite s in dbBuilding.Suites)
                    if (s.SuiteName.Equals(modelSuiteName) && !s.Deleted) { dbs = s; break; }

                if (null == dbs)
                {
                    foreach (Suite s in dbBuilding.Suites)
                        if (s.SuiteName.Equals(modelSuiteName)) { dbs = s; break; }
                }

                if (null == dbs)
                {
                    dbs = new Suite(dbBuilding, -1, Utilities.NormalizeFloorNumber(ms.Floor), modelSuiteName);
                    dbs.Status = newSuiteStatus;
                    _clientSession.DbSession.Save(dbs);
                    created = true;
                    _log.AppendFormat("Created new suite ID={0}, Name={1}\r\n", dbs.AutoID, dbs.SuiteName);
                }
                else
                {
                    if (dbs.Deleted)
                    {
                        dbs.Undelete();
                        using (SuiteDao dao = new SuiteDao(_clientSession.DbSession))
                            if (!dao.SafeUpdate(dbs)) throw new StaleObjectStateException("Suite", dbs.AutoID);
                        _log.AppendFormat("Recovered deleted suite ID={0}, Name={1}\r\n", dbs.AutoID, dbs.SuiteName);
                    }
                    else
                    {
                        _log.AppendFormat("Updating existing suite ID={0}, Name={1}\r\n", dbs.AutoID, dbs.SuiteName);
                    }
                    missingSuites.Remove(dbs.SuiteName);
                }

                importSuite(modelSite, ms, dbs, created);

				dbs.MarkUpdated();
                _clientSession.DbSession.Update(dbs);
            }

            foreach (string sn in missingSuites)
            {
                Suite dbs = null;
                foreach (Suite s in dbBuilding.Suites)
                    if (s.SuiteName.Equals(sn) && !s.Deleted) { dbs = s; break; }

                if (dbs != null)
                {
                    using (SuiteDao dao = new SuiteDao(_clientSession.DbSession))
                        if (!dao.SafeDelete(dbs)) throw new StaleObjectStateException("Suite", dbs.AutoID);
                    _log.AppendFormat("Removed suite missing in model ID={0}, Name={1}\r\n", dbs.AutoID, dbs.SuiteName);
                }
            }

            // Update wireframe file
            //
            if (infoModelFileName != null)
            {
				dbBuilding.WireframeLocation = storeModelFile(ServiceInstances.InternalFileStorageManager,
					dbBuilding, dbBuilding.WireframeLocation, infoModelFileName, "b", "wireframes");

                if (displayModelFileName != null)
					dbBuilding.DisplayModelUrl = storeModelFile(dbBuilding, dbBuilding.DisplayModelUrl, displayModelFileName, "b");

                if (overlayModelFileName != null)
					dbBuilding.OverlayModelUrl = storeModelFile(dbBuilding, dbBuilding.OverlayModelUrl, overlayModelFileName, "bo");

				// UNUSED: POI is now always on site level; 
                //if (poiModelFileName != null)
				//	dbBuilding.PoiModelUrl = storeModelFile(dbBuilding, dbBuilding.PoiModelUrl, poiModelFileName, "bp");

                if (bubbleWebTemplateFileName != null)
					dbBuilding.BubbleWebTemplateUrl = storeModelFile(dbBuilding, dbBuilding.BubbleWebTemplateUrl, bubbleWebTemplateFileName, "bbwt");

                if (bubbleKioskTemplateFileName != null)
					dbBuilding.BubbleKioskTemplateUrl = storeModelFile(dbBuilding, dbBuilding.BubbleKioskTemplateUrl, bubbleKioskTemplateFileName, "bbkt");
            }

			// Calculate and set geoinformation
			//
			var vpCenter = modelBuilding.LocationCart.AsGeoPoint();
			double mLon = 0.0, mLat = 0.0, mAlt = 0.0, maxSuiteAlt = 0.0;
			int suiteCnt = 0;
			foreach (var suiteModelInfo in modelBuilding.Suites)
			{
				var vp = suiteModelInfo.LocationCart.AsViewPoint();
				mLon += vp.Longitude;
				mLat += vp.Latitude;
				mAlt += vp.Altitude;
				if (maxSuiteAlt < vp.Altitude) maxSuiteAlt = vp.Altitude;
				suiteCnt++;
			}

			dbBuilding.Location = vpCenter;
			dbBuilding.Center = new GeoPoint(
				mLon / (double)suiteCnt,
				mLat / (double)suiteCnt,
				mAlt / (double)suiteCnt);
			dbBuilding.MaxSuiteAltitude = maxSuiteAlt;

			// Set/adjust altitude.
            //
            try
            {
                dbBuilding.AltitudeAdjustment =
                    queryForLocationAltitude(vpCenter.Longitude, vpCenter.Latitude) - vpCenter.Altitude;
            }
            catch (Exception ex)
            {
                _log.AppendFormat("ERROR: Failed querying altitude for {0}: {1}\r\n", dbBuilding.Name, ex.Message);
            }

            //if (isCreated && (infoModelFileName != null))  // new single building imported; attempt to write address
            {
				// set localized name: if provided - use it; else write it to XREF
				var locName = conditionString(_localizationXref.GetBuildingName(dbBuilding.Name), 256);
				if (string.IsNullOrEmpty(locName)) _localizationXref.WriteBuildingName(dbBuilding.Name, dbBuilding.LocalizedName);
				else dbBuilding.LocalizedName = locName;

                dbBuilding.Country = conditionString(extras.GetOption("ad_co"), 128);
                dbBuilding.PostalCode = conditionString(extras.GetOption("ad_po"), 10);
                dbBuilding.StateProvince = conditionString(extras.GetOption("ad_stpr"), 8);
                dbBuilding.City = conditionString(extras.GetOption("ad_mu"), 64);

                string rawaddr = extras.GetOption("ad_sta");
                if (null == rawaddr)
                {
                    dbBuilding.AddressLine1 = string.Empty;
                    dbBuilding.AddressLine2 = string.Empty;
                }
                else
                {
                    const int maxpartlen = 128;
                    
                    rawaddr = rawaddr.Trim();
                    int len = rawaddr.Length, len2;
                    do { len2 = len; rawaddr = rawaddr.Replace("  ", " "); len = rawaddr.Length; }
                    while (len != len2);

                    if (len <= maxpartlen)
                    {
                        dbBuilding.AddressLine1 = rawaddr;
                        dbBuilding.AddressLine2 = string.Empty;
                    }
                    else
                    {
                        int pos = -1, ppos;

                        do { ppos = pos; pos = rawaddr.IndexOf(' ', pos + 1); }
                        while ((pos > 0) && (pos < maxpartlen));
                        if (ppos > 0)
                        {
                            dbBuilding.AddressLine1 = rawaddr.Substring(0, ppos);
                            rawaddr = rawaddr.Substring(ppos + 1);
                        }
                        else
                        {
                            dbBuilding.AddressLine1 = rawaddr.Substring(0, maxpartlen);
                            rawaddr = rawaddr.Substring(maxpartlen + 1);
                        }

                        len = rawaddr.Length;
                        if (len <= maxpartlen)
                        {
                            dbBuilding.AddressLine2 = rawaddr;
                        }
                        else
                        {
                            pos = -1;
                            do { ppos = pos; pos = rawaddr.IndexOf(' ', pos + 1); }
                            while ((pos > 0) && (pos < maxpartlen));
                            if (ppos > 0)
                            {
                                dbBuilding.AddressLine2 = rawaddr.Substring(0, ppos);
                            }
                            else
                            {
                                dbBuilding.AddressLine2 = rawaddr.Substring(0, maxpartlen);
                            }
                        }
                    }
                }

                _log.AppendFormat("Effective building address is set to: {0}\r\n", 
                    AddressHelper.ConvertToReadableAddress(dbBuilding, null));
            }

			if (isCreated) importedBuildingIds.Add(dbBuilding.AutoID);

			dbBuilding.MarkUpdated();
			_clientSession.DbSession.Update(dbBuilding);
        }

		private string storeModelFile(UpdateableBase dbObject, string currentPath,
			string modelFileName, string storePrefix)
		{
			return storeModelFile(ServiceInstances.FileStorageManager, dbObject, currentPath,
				modelFileName, storePrefix, "models");
		}

        private string storeModelFile(IFileStorageManager man, UpdateableBase dbObject, string currentPath,
			string modelFileName, string storePrefix, string namespaceHint)
        {
            string result, proposedName;
			using (var fs = OpenModelFile(modelFileName, out proposedName))
			{
				if (!string.IsNullOrWhiteSpace(currentPath))
					result = man.ReplaceFile(currentPath, namespaceHint, storePrefix, 
						Path.GetExtension(proposedName), dbObject.AutoID.ToString(), fs);
				else
					result = man.StoreFile(namespaceHint, storePrefix, 
						Path.GetExtension(proposedName), dbObject.AutoID.ToString(), fs);
			}
            _filesSaved.Add(result);
            _log.AppendFormat("File persisted ({0}) from {1}\r\n", result, modelFileName);
            return result;
        }

		internal static Stream OpenModelFile(string filename, out string proposedName)
		{
			if (!Path.GetExtension(filename).Substring(1).ToUpperInvariant().Equals("KML"))
			{
				proposedName = filename;
				return File.OpenRead(filename);
			}
			else
			{
				proposedName = "generated.kmz";
				var result = new MemoryStream();
				using (var package =
						Package.Open(result, FileMode.Create))
				{
					PackagePart packagePartDocument =
						package.CreatePart(
							PackUriHelper.CreatePartUri(new Uri("default.kml", UriKind.Relative)),
							"", CompressionOption.Maximum);

					using (var fileStream = File.OpenRead(filename))
						fileStream.CopyTo(packagePartDocument.GetStream());
				}
				result.Position = 0;
				return result;
			}
		}

        internal static string conditionString(string input, int maxlen)
        {
            if (null == input) return string.Empty;
            input = input.Trim();
            if (input.Length <= maxlen) return input;
            else return input.Substring(0, maxlen);
        }

        private void importSuite(
			Vre.Server.Model.Kmz.ConstructionSite modelSite, 
			Vre.Server.Model.Kmz.Suite modelSuite, 
			Suite dbSuite, bool isCreated)
        {
			if (isCreated)
			{
				// this must be the first call on new suite as it re-reads suite from DB;
				// all subsequent changes shall be lost!
				dbSuite.CurrentPrice = new Money(Convert.ToDecimal(modelSuite.InitialPrice), _currency);

				using (SiteManager mgr = new SiteManager(_clientSession))
					mgr.LogNewSuitePrice(dbSuite, (float)modelSuite.InitialPrice, _currency);
			}

            dbSuite.ShowPanoramicView = modelSuite.ShowPanoramicView;

			// Geoinformation
			//
			dbSuite.Location = modelSuite.LocationGeo.AsViewPoint();
			dbSuite.FloorName = Utilities.NormalizeFloorNumber(modelSuite.Floor);
			dbSuite.CeilingHeight = new ValueWithUM(modelSuite.CeilingHeightFt, _heightUnit);

			// set localized name: if provided - use it; else write it to XREF
			var locName = conditionString(_localizationXref.GetSuiteName(dbSuite.SuiteName), 256);
			if (string.IsNullOrEmpty(locName)) _localizationXref.WriteSuiteName(dbSuite.SuiteName, dbSuite.LocalizedSuiteName);
			else dbSuite.LocalizedSuiteName = locName;

			locName = conditionString(_localizationXref.GetFloorName(dbSuite.FloorName), 256);
			if (string.IsNullOrEmpty(locName)) _localizationXref.WriteFloorName(dbSuite.FloorName, dbSuite.LocalizedFloorName);
			else dbSuite.LocalizedFloorName = locName;

            setSuiteType(dbSuite, modelSite, modelSuite.ClassName);
        }

        private void setSuiteType(Suite dbSuite, 
			Vre.Server.Model.Kmz.ConstructionSite modelSite,
			string newClassName)
        {
            bool newType = false, updated = false;
            SuiteType stype = null;

            string cn = newClassName;
            if (!_extraSuiteInfo.HasType(cn)) cn = dbSuite.Building.Name + "/" + cn;

            if (!_typeCache.TryGetValue(cn, out stype))
            {
                foreach (SuiteType st in dbSuite.Building.ConstructionSite.SuiteTypes)
                    if (st.Name.Equals(cn) && !st.Deleted) { stype = st; break; }

                if (null == stype)
                {
                    stype = new SuiteType(dbSuite.Building.ConstructionSite, cn);
                    newType = true;
                }

                stype.BalconyCount = _extraSuiteInfo.GetBalconyCount(cn);
                stype.TerraceCount = _extraSuiteInfo.GetTerraceCount(cn);
                stype.OtherRoomCount = _extraSuiteInfo.GetOtherRoomsCount(cn);
                stype.ShowerBathroomCount = _extraSuiteInfo.GetShowerBathroomCount(cn);
                stype.NoShowerBathroomCount = _extraSuiteInfo.GetNoShowerBathroomCount(cn);
                stype.BedroomCount = _extraSuiteInfo.GetBedroomCount(cn);
                stype.DenCount = _extraSuiteInfo.GetDenCount(cn);
                stype.FloorArea = new ValueWithUM(_extraSuiteInfo.GetIndoorFloorAreaSqFt(cn), _floorAreaUnit);

				// Add geoinformation
				//
				var wireframeModel = processGeometries(modelSite.Geometries[newClassName]);
				int idx = 0;
				ClientData[] cdmodel = new ClientData[wireframeModel.Length];
				foreach (Wireframe wf in wireframeModel) cdmodel[idx++] = wf.GetClientData();
				var cd = new ClientData();
				cd.Add("geometries", cdmodel);
				stype.WireframeModel = JavaScriptHelper.ClientDataToJson(cd);

				// set localized name: if provided - use it; else write it to XREF
				var locName = conditionString(_localizationXref.GetSuiteTypeName(stype.Name), 256);
				if (string.IsNullOrEmpty(locName))
				{
					_localizationXref.WriteSuiteTypeName(stype.Name, stype.LocalizedName);
				}
				else if (!stype.LocalizedName.Equals(locName))
				{
					stype.LocalizedName = locName;
					if (!newType) updated = true;
				}
				
				if (newType)
                {
                    _clientSession.DbSession.Save(stype);
                    _log.AppendFormat("Created new suite type ID={0}, Name={1}\r\n", stype.AutoID, stype.Name);
                }
                else
                {
                    _log.AppendFormat("Updating suite type ID={0}, Name={1}\r\n", stype.AutoID, stype.Name);
                }

                string fpName = _extraSuiteInfo.GetFloorPlanFileName(cn);
                if (!string.IsNullOrWhiteSpace(fpName) && !fpName.Equals("?"))
                {
                    string srcPath = Path.Combine(_importPath, fpName.Replace('/', '\\'));
                    if (File.Exists(srcPath))
                    {
						stype.FloorPlanUrl = storeModelFile(stype, stype.FloorPlanUrl, srcPath, "fp");

                        _clientSession.DbSession.Update(stype);
                        updated = true;
                    }
                    else
                    {
                        _log.AppendFormat("Floor plan {0} does not exist (type name={1})\r\n", srcPath, stype.Name);
                    }
                }

                if (!newType && !updated)
                    _clientSession.DbSession.Update(stype);

                _typeCache.Add(stype.Name, stype);
            }

            //dbSuite.SuiteType = stype;
            stype.debug_addSuite(dbSuite);
        }

		private static Wireframe[] processGeometries(Model.Kmz.Geometry[] geometries)
		{
			int idx = 0;
			Wireframe[] result = new Wireframe[geometries.Length];

			bool errors = false;
			foreach (Model.Kmz.Geometry geom in geometries)
			{
				if ((null == geom.Points) || (null == geom.Lines))
				{
					// TODO!!!
					errors = true;
					continue;
				}

				List<Wireframe.Point3D> points = new List<Wireframe.Point3D>(geom.Points.Count());
				foreach (Model.Kmz.Geometry.Point3D pt in geom.Points)
					points.Add(new Wireframe.Point3D(pt.X, pt.Y, pt.Z));

				List<Wireframe.Segment> segments = new List<Wireframe.Segment>(geom.Lines.Count());
				foreach (Model.Kmz.Geometry.Line ln in geom.Lines)
					segments.Add(new Wireframe.Segment(ln.Start, ln.End));

				result[idx++] = new Wireframe(points, segments);
			}
			if (idx < result.Length)  // in case we skipped something adjust array to eliminate nulls in array
			{
				Wireframe[] res_adj = new Wireframe[idx];
				Array.Copy(result, 0, res_adj, 0, idx);
				result = res_adj;
			}

			if (errors) ServiceInstances.Logger.Error("Geometry has one or more errors (model invalid); improper segments skipped.");

			return result;
		}

		internal static double queryForLocationAltitude(double longitude, double latitude)
        {
            double result;

            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(string.Format(
                    "http://maps.googleapis.com/maps/api/elevation/json?sensor=false&locations={0},{1}",
                    latitude, longitude));
                request.Timeout = 10 * 1000;

                request.Method = "GET";

                HttpWebResponse response;

                try
                {
                    response = request.GetResponse() as HttpWebResponse;
                }
                catch (WebException ex)
                {
                    response = ex.Response as HttpWebResponse;
                }
                if (null == response) throw new InvalidOperationException("Google API query returned invalid result.");

                ClientData respData = JavaScriptHelper.JsonToClientData(response.GetResponseStream());

                if (!respData.GetProperty("status", string.Empty).Equals("OK"))
                    throw new Exception("Google API query failed (0).");

                ClientData[] resultList = respData.GetNextLevelDataArray("results");
                if (resultList.Length != 1)
                    throw new Exception("Google API query failed (1).");

                double resolution = resultList[0].GetProperty("resolution", 1000000000.0);
                if (resolution > 1000.0)
                    throw new Exception("Result precision is too low.");

                result = resultList[0].GetProperty("elevation", 0.0);
            }
            catch (WebException ex)
            {
                throw new Exception("Google API query failed: " + ex.Response, ex);
            }

            return result;
        }

		//internal void tryImportExistingListings(Building building)
		//{
		//    TryImportExistingListings(_clientSession, building.AutoID, _log);
		//}

		internal static void TryImportExistingListings(ClientSession session, int buildingId, StringBuilder report)
		{
			IMlsInfoProvider prov;
			string issues;

			// STEP 1
			//
			// TODO: MLS provider injection point
			prov = new RetsMlsStatusProvider();

			prov.Configure(Configuration.Mls.Treb.Status.ConfigString.Value);
			issues = prov.Parse();
			if (issues.Length > 0) report.AppendFormat("\r\nMLS Status Retrieval problems:\r\n{0}", issues);

			var activeItems = prov.GetCurrentActiveItems();

			// STEP 2
			//
			// TODO: MLS provider injection point
			prov = new RetsMlsInfoProvider();

			prov.Configure(Configuration.Mls.Treb.Info.ConfigString.Value);

			int mlsCnt = 0, add = 0, err = 0;
			List<string> processedIds = new List<string>();
			List<string> updatedMlsInfos = new List<string>();
			foreach (var file in prov.AvailableFiles.OrderBy((a) => a.CreationTimeUtc).Reverse())
			{
				issues = prov.Parse(file.FullName);
				if (issues.Length > 0) report.AppendFormat("\r\nMLS Info Retrieval problems:\r\n{0}", issues);

				var items = prov.GetNewItems();
				for (int idx = items.Count - 1; idx >= 0; idx--)
				{
					if (!activeItems.Contains(items[idx].MlsId))
					{
						items.RemoveAt(idx);
					}
					else if (processedIds.Contains(items[idx].MlsId))
					{
						items.RemoveAt(idx);
					}
				}

				using (var manager = new SiteManager(session))
					issues = manager.RetroImportExistingViewOrders(items, buildingId, ref add, ref err, ref updatedMlsInfos);
				if (issues.Length > 0) report.AppendFormat("\r\nMLS Import problems:\r\n{0}", issues);

				foreach (var item in items) processedIds.Add(item.MlsId);

				mlsCnt += items.Count;
			}

			report.AppendFormat("\r\nUpdate completed: {0} MLS items processed; {1} ViewOrders added; {2} errors.",
				mlsCnt, add, err);
		}

		internal static void TryImportExistingListings(ClientSession session, StringBuilder report)
		{
			IMlsInfoProvider prov;
			string issues;

			// STEP 1
			//
			// TODO: MLS provider injection point
			prov = new RetsMlsStatusProvider();

			prov.Configure(Configuration.Mls.Treb.Status.ConfigString.Value);
			issues = prov.Parse();
			if (issues.Length > 0) report.AppendFormat("\r\nMLS Status Retrieval problems:\r\n{0}", issues);

			var activeItems = prov.GetCurrentActiveItems();

			// STEP 2
			//
			// TODO: MLS provider injection point
			prov = new RetsMlsInfoProvider();

			prov.Configure(Configuration.Mls.Treb.Info.ConfigString.Value);

			IDictionary<Guid, string> voIds;
			using (var vodao = new ViewOrderDao(session.DbSession))
				// TODO: MLS provider filter injection point
				voIds = vodao.GetAllActiveIdsAndMlsIdV2();

			int mlsCnt = 0, add = 0, err = 0;
			List<string> processedIds = new List<string>();
			List<string> updatedMlsInfos = new List<string>();
			foreach (var file in prov.AvailableFiles.OrderBy((a) => a.CreationTimeUtc).Reverse())
			{
				issues = prov.Parse(file.FullName);
				if (issues.Length > 0) report.AppendFormat("\r\nMLS Info Retrieval problems:\r\n{0}", issues);

				var items = prov.GetNewItems();
				for (int idx = items.Count - 1; idx >= 0; idx--)
				{
					if (!activeItems.Contains(items[idx].MlsId))
					{
						items.RemoveAt(idx);
					}
					else if (processedIds.Contains(items[idx].MlsId))
					{
						items.RemoveAt(idx);
					}
				}

				using (var manager = new SiteManager(session))
					issues = manager.RetroImportExistingViewOrders(items, ref voIds, ref add, ref err, ref updatedMlsInfos);
				if (issues.Length > 0) report.AppendFormat("\r\nMLS Import problems:\r\n{0}", issues);

				foreach (var item in items) processedIds.Add(item.MlsId);

				mlsCnt += items.Count;
			}

			report.AppendFormat("\r\nUpdate completed: {0} MLS items processed; {1} ViewOrders added; {2} errors.",
				mlsCnt, add, err);
		}
	}
}