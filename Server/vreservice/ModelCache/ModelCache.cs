using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Vre.Server.BusinessLogic;

namespace Vre.Server.ModelCache
{
    internal class ModelCache
    {
        private SHA512 _lastCrc;
        private SiteInfo _info;

        public ModelCache(string filePath)
        {
            _info = parseFile(filePath);
            if (_info != null)
            {
                _lastCrc = calcCrc(filePath);
                FilePath = filePath;
                UpdatedTime = File.GetLastWriteTimeUtc(filePath);
            }
        }

        public bool IsValid { get { return _info != null; } }
        public string FilePath { get; private set; }
        public DateTime UpdatedTime { get; private set; }

        public bool TryReRead(string filePath)
        {
            bool result = false;
            SHA512 newCrc = calcCrc(filePath);

            if (!newCrc.Equals(_lastCrc))
            {
                SiteInfo si = parseFile(filePath);
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

        public string SiteName { get { if (_info != null) return _info.Name; else return null; } }

        public void UpdateBo(Site target, bool withSubObjects)
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

        private static SiteInfo parseFile(string filePath)
        {
            SiteInfo result = null;

            try
            {
                VrEstate.Site siteData;

                using (VrEstate.Kmz kmz = VrEstate.Kmz.Open(filePath, System.IO.FileAccess.Read))
                {
                    VrEstate.Model.Setup(kmz.GetKmlDoc());
                    siteData = new VrEstate.Site(kmz.GetColladaDoc());
                }

                result = new SiteInfo(siteData, filePath);
            }
            catch (Exception ex)
            {
                ServiceInstances.Logger.Error("Cannot parse model file \"{0}\":\r\n{1}", filePath, Utilities.ExplodeException(ex));
            }

            return result;
        }

        internal class SiteInfo
        {
            public string Name;

            private GeoPoint _location;

            private Dictionary<string, BuildingInfo> _buildingInfo;

            public SiteInfo(VrEstate.Site modelInfo, string path)
            {
                //Name = modelInfo.Name;
                string name = Path.GetFileName(path);
                int pos = name.IndexOf('.');
                if (pos > 0) name = name.Substring(0, pos - 1);
                pos = name.IndexOf('-');
                if (pos > 0) name = name.Substring(0, pos - 1);
                Name = name.Trim();

                _location = new GeoPoint(modelInfo.Lon_d, modelInfo.Lat_d, modelInfo.Alt_m);

                //modelInfo.DirName  // null
                //modelInfo.ID  // nine-digit-int

                _buildingInfo = new Dictionary<string, BuildingInfo>(modelInfo.Buildings.Values.Count);
                foreach (VrEstate.Building buildingModelInfo in modelInfo.Buildings.Values)
                {
                    BuildingInfo bi = new BuildingInfo(buildingModelInfo);

                    if (_buildingInfo.ContainsKey(bi.Name))
                    {
                        ServiceInstances.Logger.Error("Model contains duplicate building names: '{0}'", bi.Name);
                        continue;
                    }

                    _buildingInfo.Add(bi.Name, bi);
                }
            }

            public void UpdateBo(Site target, bool withSubObjects)
            {
                target.Location = _location;

                if (withSubObjects)
                {
                    int toUpdate = target.Buildings.Count;
                    foreach (string buildingName in _buildingInfo.Keys)
                    {
                        var buildings = from bd in target.Buildings where bd.Name == buildingName select bd;
                        if (1 == buildings.Count())
                        {
                            Building buildingBo = buildings.First();
                            _buildingInfo[buildingName].UpdateBo(buildingBo, true);
                            toUpdate--;
                        }
                    }

                    Debug.Assert(toUpdate == 0, "Not all buildings were updated from model information.");
                }
            }

            public bool UpdateBo(Building target, bool withSubObjects)
            {
                BuildingInfo bi;
                if (_buildingInfo.TryGetValue(target.Name, out bi))
                {
                    bi.UpdateBo(target, withSubObjects);
                    return true;
                }
                return false;
            }

            public bool UpdateBo(Suite target, bool withSubObjects)
            {
                BuildingInfo bi;
                if (_buildingInfo.TryGetValue(target.Building.Name, out bi))
                {
                    return bi.UpdateBo(target, withSubObjects);
                }
                return false;
            }
        }

        internal class BuildingInfo
        {
            public string Name;

            private GeoPoint _location;

            private Dictionary<string, SuiteInfo> _suiteInfo;

            public BuildingInfo(VrEstate.Building modelInfo)
            {
                Name = modelInfo.Name;

                _location = new GeoPoint(modelInfo.Lon_d, modelInfo.Lat_d, modelInfo.Alt_m);

                //modelInfo.BuildingId  // "ID<five-digit-int>"
                //modelInfo.ID          // nine-digit-int
                //modelInfo.MaxAlt_m

                _suiteInfo = new Dictionary<string, SuiteInfo>(modelInfo.Suites.Values.Count);
                foreach (VrEstate.Suite suiteModelInfo in modelInfo.Suites.Values)
                {
                    SuiteInfo si = new SuiteInfo(suiteModelInfo);

                    if (_suiteInfo.ContainsKey(si.Name))
                    {
                        ServiceInstances.Logger.Error("Model contains duplicate suite names within building: '{0}'", si.Name);
                        continue;
                    }

                    _suiteInfo.Add(si.Name, si);
                }
            }

            public void UpdateBo(Building target, bool withSubObjects)
            {
                target.Location = _location;

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
            private Wireframe[] _wireframe;
            private string _classId;
            private double _ceilingHeightFt;
            private string _floor;

            public SuiteInfo(VrEstate.Suite modelInfo)
            {
                Name = modelInfo.Name;

                _classId = modelInfo.ClassId;
                _ceilingHeightFt = modelInfo.CellingHeight;
                _location = new ViewPoint(modelInfo.Lon_d, modelInfo.Lat_d, modelInfo.Alt_m, modelInfo.Heading_d);
                _wireframe = processGeometries(modelInfo.Geometries);
                _floor = modelInfo.FloorNumber;

                //modelInfo.Id;  // "ID<five-digit-int>"
            }

            private static Wireframe[] processGeometries(VrEstate.Geometry[] geometries)
            {
                int idx = 0;
                Wireframe[] result = new Wireframe[geometries.Length];

                foreach (VrEstate.Geometry geom in geometries)
                {
                    List<Wireframe.Point3D> points = new List<Wireframe.Point3D>(geom.Points.Length);
                    foreach (VrEstate.Geometry.Point3 pt in geom.Points)
                        points.Add(new Wireframe.Point3D(pt.X, pt.Y, pt.Z));

                    List<Wireframe.Segment> segments = new List<Wireframe.Segment>(geom.Lines.Count);
                    foreach (VrEstate.Geometry.Line ln in geom.Lines)
                        segments.Add(new Wireframe.Segment(ln.Start, ln.End));

                    result[idx++] = new Wireframe(points, segments);
                }

                return result;
            }

            public void UpdateBo(Suite target, bool withSubObjects)
            {
                target.CeilingHeight = new ValueWithUM(_ceilingHeightFt, ValueWithUM.Unit.Feet);
                ////target.SuiteType
                target.FloorName = _floor;
                target.Location = _location;
                target.Model = _wireframe;
            }
        }
    }
}