using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ModelReaderTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Vre.Server.Model.Kmz.ViewPoint vp1 = new Vre.Server.Model.Kmz.ViewPoint();
            //vp1.Longitude = -79.5;
            //vp1.Latitude = 48.3;
            //vp1.Altitude = 0.0;

            //Vre.Server.Model.Kmz.EcefViewPoint evp = new Vre.Server.Model.Kmz.EcefViewPoint(vp1);

            //Vre.Server.Model.Kmz.ViewPoint vp2 = evp.AsViewPoint();

            //vp1.Longitude = -83.1;
            //vp1.Latitude = 37.2;
            //vp1.Altitude = 63.0;

            //evp = new Vre.Server.Model.Kmz.EcefViewPoint(vp1);
            //vp2 = evp.AsViewPoint();

            //System.Diagnostics.Debugger.Break();

            string modelFileName;
            if (args.Length > 1) return;
            if (args.Length < 1)
                //modelFileName = @"C:\Internet Downloads\Business\Resale\Test Model - Dual.kmz";
                modelFileName = @"C:\Internet Downloads\Business\Devonwood\Devonwood fixed.kmz";
            //modelFileName = @"C:\Documents and Settings\port443\My Documents\My Dropbox\AndrewShare\Eden Park Towers (Phase II) - Read Only.kmz";
            //modelFileName = @"C:\Documents and Settings\port443\My Documents\My Dropbox\AndrewShare\SuperServer\Times Group Corporation\Eden_Park_Towers_Phase_II\Model.kmz";
            else
                modelFileName = args[0];

            StringBuilder readWarnings = new StringBuilder();
            Vre.Server.Model.Kmz.Kmz kmzx = new Vre.Server.Model.Kmz.Kmz(modelFileName, readWarnings);

            generateCoordinateKml(kmzx, 0);

            //System.Diagnostics.Debugger.Break();

            VrEstate.Site siteData;
            VrEstate.Model model = new VrEstate.Model();

            Console.Write("Reading {0}...", modelFileName);

            try
            {
                using (VrEstate.Kmz kmz = VrEstate.Kmz.Open(modelFileName, System.IO.FileAccess.Read))
                {
                    VrEstate.Model.Setup(kmz.GetKmlDoc());
                    siteData = new VrEstate.Site(kmz.GetColladaDoc());
                }

                StringBuilder warnings = new StringBuilder();
                Vre.Server.Model.Kmz.Kmz kmz2 = new Vre.Server.Model.Kmz.Kmz(modelFileName, warnings);

                kmz2.Equals(siteData);

                Console.Write("\r\n\r\nSite information:" +
                    "\r\n- name: {0}" +
                    "\r\n- path: {1}" +
                    "\r\n- ID: {2}" +
                    "\r\n- item count: {3}" +
                    "\r\n- coordinates: {4}, {5}; {6}m",
                    siteData.Name,
                    siteData.DirName,
                    siteData.ID,
                    siteData.HowManyItems,
                    siteData.Lon_d, siteData.Lat_d, siteData.Alt_m);


                foreach (var bldg in siteData.Buildings.Values)
                {
                    using (FileStream file = File.Create(bldg.Name + ".sql"))
                    {
                        using (StreamWriter sw = new StreamWriter(file))
                        {
                            sw.WriteLine("declare @buildingid int");
                            sw.WriteLine("set @buildingid = -1");
                            foreach (var s in bldg.Suites.Values)
                            {
                                sw.WriteLine("insert into [cmpt_SuiteClassName] ([buildingid],[suitename],[suiteclass]) values(@buildingid,'{0}','{1}')",
                                    s.Name, s.ClassId);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write("\r\n\r\nError: {0}\r\n{1}", ex.Message, ex.StackTrace);
            }

            Console.Write("\r\n\r\nPress any key to exit.");
            Console.ReadKey(false);
        }

        private static void generateCoordinateKml(Vre.Server.Model.Kmz.Kmz readModel, double altAdj)
        {
            using (FileStream file = File.Create("coordinates-debug.kml"))
            {
                using (StreamWriter sw = new StreamWriter(file))
                {
                    sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    sw.WriteLine("<kml xmlns=\"http://earth.google.com/kml/2.0\">");
                    sw.WriteLine("<Document>");
                    sw.WriteLine("<name>{0}</name>", readModel.Name);
                    sw.WriteLine("<description>Debug output of coordinate transformer</description>");

                    sw.WriteLine("<Style id=\"s1\">");
                    sw.WriteLine("<LineStyle><color>ffff0000</color><width>2</width></LineStyle>");
                    sw.WriteLine("</Style>");

                    sw.WriteLine("<Style id=\"s2\">");
                    sw.WriteLine("<LineStyle><color>ffff8800</color><width>2</width></LineStyle>");
                    sw.WriteLine("</Style>");

                    sw.WriteLine("<Style id=\"s3\">");
                    sw.WriteLine("<LineStyle><color>ffffff00</color><width>2</width></LineStyle>");
                    sw.WriteLine("</Style>");

                    sw.WriteLine("<Style id=\"s4\">");
                    sw.WriteLine("<LineStyle><color>ff00ff00</color><width>4</width></LineStyle>");
                    sw.WriteLine("</Style>");

                    sw.WriteLine("<Style id=\"s5\">");
                    sw.WriteLine("<LineStyle><color>ff0000ff</color><width>4</width></LineStyle>");
                    sw.WriteLine("</Style>");

                    sw.WriteLine("<Style id=\"s6\">");
                    sw.WriteLine("<LineStyle><color>ff0088ff</color><width>3</width></LineStyle>");
                    sw.WriteLine("</Style>");


                    /*
        <Placemark> 
                <name>My Path</name> 
    <styleUrl>#myStyle</styleUrl> 
    <MultiGeometry> 
                <LineString> 
                        <tessellate>1</tessellate> 
                        <coordinates> 
-107.0303781250365,30.27056500199735,0 
-106.6109752769761,30.27616399690955,0 
-106.0800016002764,30.25957244616284,0 </coordinates> 
                </LineString> 
    <Point> 
    <coordinates>-106.6109752769761,30.27616399690955,0</coordinates> 
    </Point> 
    </MultiGeometry> 
        </Placemark> 
                     */
                    writePlacemark(sw, "s1", "Base", "Construction site",
                        readModel.Model.Location, readModel.Model.Site.LocationCart.AsViewPoint(), altAdj);

                    foreach (Vre.Server.Model.Kmz.Building bldg in readModel.Model.Site.Buildings)
                    {
                        writePlacemark(sw, "s2", bldg.Name, "Building",
                            readModel.Model.Site.LocationCart, bldg.LocationCart, altAdj);

                        foreach (Vre.Server.Model.Kmz.Suite s in bldg.Suites)
                        {
                            writePlacemark(sw, "s3", s.Name, "Suite",
                                bldg.LocationCart, s.LocationCart, altAdj);

                            Vre.Server.Model.Kmz.Geometry[] geo;
                            if (readModel.Model.Site.Geometries.TryGetValue(s.ClassName, out geo))
                                writeGeometry(sw, "s6", s.LocationCart, geo, s.Matrix, altAdj);
                            //foreach (string id in s.GeometryIdList)
                            //{
                            //    Vre.Server.Model.Kmz.Geometry[] geo;
                            //    if (readModel.Model.Site.Geometries.TryGetValue(id, out geo))
                            //        writeGeometry(sw, "s6", s.LocationCart, geo, altAdj);
                            //}
                        }
                    }

                    sw.WriteLine("</Document>");
                    sw.WriteLine("</kml>");
                }
            }
        }

        private static void writePlacemark(StreamWriter sw, string styleUrl,
            string name, string description, Vre.Server.Model.Kmz.EcefViewPoint from, Vre.Server.Model.Kmz.EcefViewPoint to,
            double altAdj)
        {
            writePlacemark(sw, styleUrl, name, description, from.AsViewPoint(), to.AsViewPoint(), altAdj);
        }

        private static void writePlacemark(StreamWriter sw, string styleUrl,
            string name, string description, Vre.Server.Model.Kmz.ViewPoint from, Vre.Server.Model.Kmz.ViewPoint to,
            double altAdj)
        {
            sw.WriteLine("<Placemark>");

            if (styleUrl != null) sw.WriteLine("<styleUrl>{0}</styleUrl>", styleUrl);
            if (name != null) sw.WriteLine("<name>{0}</name>", name);
            if (description != null) sw.WriteLine("<description>{0}</description>", description);

            sw.WriteLine("<Point><altitudeMode>relativeToGround</altitudeMode><coordinates>");
            sw.WriteLine(viewPointToKmlNotation(to, altAdj));
            sw.WriteLine("</coordinates></Point>");

//            sw.WriteLine("<MultiGeometry>");
            sw.WriteLine("<LineString><altitudeMode>relativeToGround</altitudeMode><coordinates>");
            sw.WriteLine("{0}{1}{2},{3},{4}",
                viewPointToKmlNotation(from, altAdj), viewPointToKmlNotation(to, altAdj),
                to.Longitude, to.Latitude, to.Altitude + 2.0 + altAdj);
            sw.WriteLine("</coordinates></LineString>");
//            sw.WriteLine("</MultiGeometry>");

            sw.WriteLine("</Placemark>");

            double lon, lat, alt;
            string style;
            if ((to.Heading >= 0.0) && (to.Heading < 360.0))
            {
                lon = to.Longitude - Math.Sin(to.Heading * Vre.Server.Model.Kmz.EcefViewPoint.DegreesToRad) * 0.0001;
                lat = to.Latitude + Math.Cos(to.Heading * Vre.Server.Model.Kmz.EcefViewPoint.DegreesToRad) * 0.0001;
                alt = to.Altitude + altAdj;
                style = "s4";
            }
            else
            {
                lon = to.Longitude;
                lat = to.Latitude;
                alt = to.Altitude + 4.0 + altAdj;
                style = "s5";
            }
            sw.WriteLine("<Placemark>");
            if (styleUrl != null) sw.WriteLine("<styleUrl>{0}</styleUrl>", style);

            sw.WriteLine("<LineString><altitudeMode>relativeToGround</altitudeMode><coordinates>");
            sw.WriteLine("{0},{1},{2} {3}", lon, lat, alt, viewPointToKmlNotation(to, altAdj));
            sw.WriteLine("</coordinates></LineString>");

            sw.WriteLine("</Placemark>");
        }

        private static string viewPointToKmlNotation(Vre.Server.Model.Kmz.ViewPoint vp, double altAdj)
        {
            return string.Format("{0},{1},{2} ", vp.Longitude, vp.Latitude, vp.Altitude + altAdj);
        }

        private static void writeGeometry(
            StreamWriter sw, 
            string styleUrl,
            Vre.Server.Model.Kmz.EcefViewPoint basePoint,
            Vre.Server.Model.Kmz.Geometry[] data,
            Vre.Server.Model.Kmz.TMatrix matrix,
            double altAdj)
        {
            foreach (Vre.Server.Model.Kmz.Geometry g in data)
            {
                Vre.Server.Model.Kmz.Geometry.Point3D[] pts = g.Points.ToArray();
                foreach (Vre.Server.Model.Kmz.Geometry.Line l in g.Lines)
                {
                    Vre.Server.Model.Kmz.Geometry.Point3D s = pts[l.Start];
                    Vre.Server.Model.Kmz.Geometry.Point3D ss = matrix.Transform(s);
                    Vre.Server.Model.Kmz.ViewPoint vpS =
                        matrix.Point3D2ViewPoint(ss, basePoint.Base);

                    Vre.Server.Model.Kmz.Geometry.Point3D e = pts[l.End];
                    Vre.Server.Model.Kmz.Geometry.Point3D ee = matrix.Transform(e);
                    Vre.Server.Model.Kmz.ViewPoint vpE =
                        matrix.Point3D2ViewPoint(ee, basePoint.Base);

                    sw.WriteLine("<Placemark>");

                    if (styleUrl != null) sw.WriteLine("<styleUrl>{0}</styleUrl>", styleUrl);

                    sw.WriteLine("<LineString><altitudeMode>relativeToGround</altitudeMode><coordinates>");
                    sw.WriteLine("{0}{1}",
                        viewPointToKmlNotation(vpS, altAdj),
                        viewPointToKmlNotation(vpE, altAdj));
                    sw.WriteLine("</coordinates></LineString>");

                    sw.WriteLine("</Placemark>");
                }
            }
        }
    }
}
