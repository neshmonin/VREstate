using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;

namespace Vre.Server.ModelCache
{
    internal class ModelCache
    {
        public enum ModelLevel { Site, Building }

        private SHA512 _lastCrc;
        private SiteInfo _info;

        public ModelCache(string filePath, ModelLevel level, bool isOverride, int objectId)
        {
            _info = parseFile(filePath, objectId, level);
            if (_info != null)
            {
                _lastCrc = calcCrc(filePath);
                FilePath = filePath;
                UpdatedTime = File.GetLastWriteTimeUtc(filePath);
                IsOverride = isOverride;
                Level = level;
                ObjectId = objectId;
            }
        }

        public bool IsOverride { get; private set; }
        public bool IsValid { get { return _info != null; } }
        public string FilePath { get; private set; }
        public DateTime UpdatedTime { get; private set; }
        public ModelLevel Level { get; private set; }
        public int ObjectId { get; private set; }

        public bool TryReRead(string filePath)
        {
            bool result = false;
            SHA512 newCrc = calcCrc(filePath);

            if (!newCrc.Equals(_lastCrc))
            {
                SiteInfo si = parseFile(filePath, ObjectId, Level);
                if (si != null)
                {
                    _info = si;
                    _lastCrc = newCrc;
                    FilePath = filePath;
                    UpdatedTime = File.GetLastWriteTimeUtc(filePath);
                    result = true;
                }
                else
                {
                    ServiceInstances.Logger.Error("Model reading (\"{0}\") failed; previously cached model is preserved.",
                        filePath);
                }
            }

            return result;
        }

        public void MergeSuiteTypes(ModelCache merger)
        {
            _info.MergeSuiteClassInfos(merger._info);
        }

        public void UpdateBo(Site target, bool withSubObjects)
        {
            if (_info != null)
            {
                _info.UpdateBo(target, withSubObjects);
            }
        }

        public void UpdateBo(SuiteType target, bool withSubObjects)
        {
            if (_info != null)
            {
                _info.UpdateBo(target, withSubObjects);
            }
        }

        public void UpdateBo(Building target, bool withSubObjects)
        {
            if (_info != null)
            {
                _info.UpdateBo(target, withSubObjects);
            }
        }

        public void UpdateBo(Suite target, bool withSubObjects)
        {
            if (_info != null)
            {
                _info.UpdateBo(target, withSubObjects);
            }
        }

        private static SHA512 calcCrc(string filePath)
        {
            SHA512 result = SHA512.Create();

            using (var stream = new BufferedStream(File.OpenRead(filePath), 65536))
            {
                result.ComputeHash(stream);
            }

            return result;
        }

        private static SiteInfo parseFile(string filePath, int objectId, ModelLevel level)
        {
            SiteInfo result = null;

            try
            {
                StringBuilder readWarnings = new StringBuilder();

                Model.Kmz.Kmz kmz = new Model.Kmz.Kmz(filePath, readWarnings);

                result = new SiteInfo(kmz.Model.Site, filePath, objectId, (level == ModelLevel.Building));
                // see ModelCacheManager.objectFromPath for extending here
            }
            catch (Exception ex)
            {
                ServiceInstances.Logger.Error("Cannot parse model file \"{0}\":\r\n{1}", filePath, Utilities.ExplodeException(ex));
            }

            return result;
        }

        internal class SiteInfo
        {
            private GeoPoint _location;
            private Dictionary<int, BuildingInfo> _buildingInfo;
            private Dictionary<string, SuiteClassInfo> _classInfo;

            public SiteInfo(Model.Kmz.ConstructionSite modelInfo, string path, int objectId, bool singleBuilding)
            {
                //Name = modelInfo.Name;
                //string name = Path.GetFileName(path);
                //int pos = name.IndexOf('.');
                //if (pos > 0) name = name.Substring(0, pos - 1);
                //pos = name.IndexOf('-');
                //if (pos > 0) name = name.Substring(0, pos - 1);
                //Name = name.Trim();

                Model.Kmz.ViewPoint vp = modelInfo.LocationCart.AsViewPoint();
                _location = new GeoPoint(vp.Longitude, vp.Latitude, vp.Altitude);
                //modelInfo.DirName  // null
                //modelInfo.ID  // nine-digit-int

                _buildingInfo = new Dictionary<int, BuildingInfo>(modelInfo.Buildings.Count());
                _classInfo = new Dictionary<string, SuiteClassInfo>();

                using (NHibernate.ISession dbSession = NHibernateHelper.GetSession())
                {
                    if (singleBuilding)
                    {
                        Building building;
                        using (BuildingDao dao = new BuildingDao(dbSession)) building = dao.GetById(objectId);
                        if (null == building)
                        {
                            ServiceInstances.Logger.Error("Unknown building ID used in model path: {0}", objectId);
                            throw new ArgumentException("Unknown building ID");
                        }

                        // find related model building object by name from DB
                        //
                        Model.Kmz.Building buildingModelInfo = null;
                        // TODO: MODEL-DB TEXT COMPARISON
                        // Here we match model's building name against its name in DB
                        foreach (Model.Kmz.Building b in modelInfo.Buildings)
                            if (b.Name.Equals(building.Name)) { buildingModelInfo = b; break; }
                        if (null == buildingModelInfo)
                        {
                            ServiceInstances.Logger.Error("Unknown building name; model {0} does not have {1}; building not imported.",
                                path, building.Name);
                            return;
                        }

                        BuildingInfo bi = new BuildingInfo(buildingModelInfo);
                        _buildingInfo.Add(building.AutoID, bi);

                        foreach (string className in modelInfo.Geometries.Keys)
                        {
                            SuiteClassInfo sc = new SuiteClassInfo(processGeometries(modelInfo.Geometries[className]));
                            string cn = className;
                            if (!cn.Contains('/')) cn = buildingModelInfo.Name + '/' + className;
                            _classInfo.Add(cn, sc);
                        }
                    }
                    else  // full site processing (!singleBuilding)
                    {
                        // find related DB site object by passed ID (from model's file path)
                        //
                        Site site;
                        using (SiteDao dao = new SiteDao(dbSession)) site = dao.GetById(objectId);
                        if (null == site)
                        {
                            ServiceInstances.Logger.Error("Unknown site ID used in model path: {0}", objectId);
                            throw new ArgumentException("Unknown site ID");
                        }

                        foreach (Model.Kmz.Building buildingModelInfo in modelInfo.Buildings)
                        {
                            // find related DB building object by name from model
                            //
                            Building building = null;
                            // TODO: MODEL-DB TEXT COMPARISON
                            // Here we match model's building name against its name in DB
                            foreach (Building b in site.Buildings) if (b.Name.Equals(buildingModelInfo.Name)) { building = b; break; }
                            if (null == building)
                            {
                                ServiceInstances.Logger.Error("Unknown building name used in model for {0} ({1}): {2}; building from model is skipped.",
                                    site.Name, site.AutoID, buildingModelInfo.Name);
                                continue;
                            }

                            if (_buildingInfo.ContainsKey(building.AutoID))
                            {
                                ServiceInstances.Logger.Error("Model ({0} ({1})) contains duplicate building names: '{2}'; second occurrence is skipped.",
                                    site.Name, site.AutoID, buildingModelInfo.Name);
                                continue;
                            }

                            BuildingInfo bi = new BuildingInfo(buildingModelInfo);
                            _buildingInfo.Add(building.AutoID, bi);
                        }

                        foreach (string className in modelInfo.Geometries.Keys)
                        {
                            SuiteClassInfo sc = new SuiteClassInfo(processGeometries(modelInfo.Geometries[className]));
                            _classInfo.Add(className, sc);
                        }
                    }  // site-level import
                }
            }

            public void MergeSuiteClassInfos(SiteInfo merger)
            {
                foreach (KeyValuePair<string, SuiteClassInfo> kvp in merger._classInfo)
                {
                    if (_classInfo.ContainsKey(kvp.Key)) _classInfo[kvp.Key] = kvp.Value;
                    else _classInfo.Add(kvp.Key, kvp.Value);
                }
            }

            private static Wireframe[] processGeometries(Model.Kmz.Geometry[] geometries)
            {
                int idx = 0;
                Wireframe[] result = new Wireframe[geometries.Length];

                foreach (Model.Kmz.Geometry geom in geometries)
                {
                    List<Wireframe.Point3D> points = new List<Wireframe.Point3D>(geom.Points.Count());
                    foreach (Model.Kmz.Geometry.Point3D pt in geom.Points)
                        points.Add(new Wireframe.Point3D(pt.X, pt.Y, pt.Z));

                    List<Wireframe.Segment> segments = new List<Wireframe.Segment>(geom.Lines.Count());
                    foreach (Model.Kmz.Geometry.Line ln in geom.Lines)
                        segments.Add(new Wireframe.Segment(ln.Start, ln.End));

                    result[idx++] = new Wireframe(points, segments);
                }

                return result;
            }

            public void UpdateBo(Site target, bool withSubObjects)
            {
                target.Location = _location;

                if (withSubObjects)
                {
                    int toUpdate = target.Buildings.Count;
                    foreach (Building building in target.Buildings)
                    {
                        BuildingInfo bi;
                        if (_buildingInfo.TryGetValue(building.AutoID, out bi))
                        {
                            bi.UpdateBo(building, true);
                            toUpdate--;
                        }
                    }

                    Debug.Assert(toUpdate == 0, "Not all buildings were updated from model information.");
                }
            }

            public void UpdateBo(SuiteType target, bool withSubObjects)
            {
                SuiteClassInfo sc;
                if (_classInfo.TryGetValue(target.Name, out sc))
                {
                    sc.UpdateBo(target);
                }
            }

            public bool UpdateBo(Building target, bool withSubObjects)
            {
                BuildingInfo bi;
                if (_buildingInfo.TryGetValue(target.AutoID, out bi))
                {
                    bi.UpdateBo(target, withSubObjects);
                    return true;
                }
                return false;
            }

            public bool UpdateBo(Suite target, bool withSubObjects)
            {
                BuildingInfo bi;
                if (_buildingInfo.TryGetValue(target.Building.AutoID, out bi))
                {
                    return bi.UpdateBo(target, withSubObjects);
                }
                return false;
            }
        }

        internal class BuildingInfo
        {
            private GeoPoint _location, _center;
            private double _maxSuiteAlt;
            private Dictionary<string, SuiteInfo> _suiteInfo;

            public BuildingInfo(Model.Kmz.Building modelInfo)
            {
                //Name = modelInfo.Name;
                Model.Kmz.ViewPoint vp = modelInfo.LocationCart.AsViewPoint();
                _location = new GeoPoint(vp.Longitude, vp.Latitude, vp.Altitude);
                _maxSuiteAlt = 0.0;
                //modelInfo.BuildingId  // "ID<five-digit-int>"
                //modelInfo.ID          // nine-digit-int
                //modelInfo.MaxAlt_m

                double mLon = 0.0, mLat = 0.0, mAlt = 0.0;
                int suiteCnt = 0;

                _suiteInfo = new Dictionary<string, SuiteInfo>(modelInfo.Suites.Count());
                foreach (Model.Kmz.Suite suiteModelInfo in modelInfo.Suites)
                {
                    SuiteInfo si = new SuiteInfo(suiteModelInfo);

                    if (_suiteInfo.ContainsKey(si.Name))
                    {
                        ServiceInstances.Logger.Error("Model contains duplicate suite names within building: '{0}'", si.Name);
                        continue;
                    }

                    _suiteInfo.Add(si.Name, si);

                    vp = suiteModelInfo.LocationCart.AsViewPoint();
                    mLon += vp.Longitude;
                    mLat += vp.Latitude;
                    mAlt += vp.Altitude;
                    if (_maxSuiteAlt < vp.Altitude) _maxSuiteAlt = vp.Altitude;
                    suiteCnt++;
                }
                _center = new GeoPoint(
                    mLon / (double)suiteCnt,
                    mLat / (double)suiteCnt,
                    mAlt / (double)suiteCnt);
            }

            public void UpdateBo(Building target, bool withSubObjects)
            {
                target.Location = _location;
                target.Center = _center;
                target.MaxSuiteAltitude = _maxSuiteAlt;

                if (withSubObjects)
                {
                    int toUpdate = target.Suites.Count;
                    foreach (string suiteName in _suiteInfo.Keys)
                    {
                        var suites = from st in target.Suites where st.SuiteName == suiteName select st;
                        if (1 == suites.Count())
                        {
                            Suite suiteBo = suites.First();
                            _suiteInfo[suiteName].UpdateBo(suiteBo, true);
                            toUpdate--;
                        }
                    }

                    Debug.Assert(toUpdate == 0, "Not all suites were updated from model information.");
                }
            }

            public bool UpdateBo(Suite target, bool withSubObjects)
            {
                SuiteInfo bi;
                if (_suiteInfo.TryGetValue(target.SuiteName, out bi))
                {
                    bi.UpdateBo(target, withSubObjects);
                    return true;
                }
                return false;
            }
        }

        internal class SuiteInfo
        {
            public string Name;

            private ViewPoint _location;
            private string _classId;
            private double _ceilingHeightFt;
            private string _floor;

            public SuiteInfo(Model.Kmz.Suite modelInfo)
            {
                Name = modelInfo.Name;

                _classId = modelInfo.ClassName;
                _ceilingHeightFt = modelInfo.CeilingHeightFt;
                Model.Kmz.ViewPoint vp = modelInfo.LocationGeo;// modelInfo.LocationCart.AsViewPoint();

                // Client's heading issue patch
                //vp.Heading += 90.0;
                //if (vp.Heading >= 180.0) vp.Heading -= 360.0;

                _location = new ViewPoint(vp.Longitude, vp.Latitude, vp.Altitude, vp.Heading);
                _floor = modelInfo.Floor;

                //modelInfo.Id;  // "ID<five-digit-int>"
            }

            public void UpdateBo(Suite target, bool withSubObjects)
            {
                target.CeilingHeight = new ValueWithUM(_ceilingHeightFt, ValueWithUM.Unit.Feet);
                ////target.SuiteType
                target.FloorName = _floor;
                target.Location = _location;
                //target.ClassName = _classId;
            }
        }

        internal class SuiteClassInfo
        {
            private Wireframe[] _model;

            public SuiteClassInfo(Wireframe[] model)
            {
                _model = model;
            }

            public void UpdateBo(SuiteType target)
            {
                target.WireframeModel = _model;
            }
        }
    }
}