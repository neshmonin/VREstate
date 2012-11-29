using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Vre.Server.Model;
using Vre.Server.Model.Kmz;

namespace ModelPackageTester
{
    public partial class Form1 : Form
    {
        private string _modelFileName = null;
        private string _stiFileName = null;
        private string _floorPlanPath = null;

        private string doTest()
        {
            StringBuilder readWarnings = new StringBuilder();

            // Parse KMZ into object model
            //
            Kmz kmzx = new Kmz(_modelFileName, readWarnings);

            // Read in CSV
            //
            CsvSuiteTypeInfo info = new CsvSuiteTypeInfo(_stiFileName, readWarnings);

            // Test floor plan files presense
            //
            foreach (string stn in info.TypeInfoList)
            {
                string file = info.GetFloorPlanFileName(stn);
                if (!string.IsNullOrWhiteSpace(file))
                {
                    file = Path.Combine(_floorPlanPath, file);
                    if (!File.Exists(file))
                        readWarnings.AppendFormat("\r\nFPFS00: Suite type {0} lists floor plan {1} which does not exist.", stn, file);
                }
                else
                {
                    readWarnings.AppendFormat("\r\nSTMD00: Suite type {0} lists no floor plan.", stn);
                }
            }

            // Test common cross-reference issues
            //
            HashSet<string> missingTypes = new HashSet<string>();
            HashSet<string> passedTypes = new HashSet<string>();

            foreach (Building b in kmzx.Model.Site.Buildings)
            {
                foreach (Suite s in b.Suites)
                {
                    if (info.HasType(s.ClassName))
                    {
                        if (missingTypes.Add(s.ClassName))
                            readWarnings.AppendFormat("\r\nSTMD01: Suite type {0} in model file has no related entry in SuiteTypeInfo.", s.ClassName);
                    }
                    else if (passedTypes.Add(s.ClassName))
                    {
                        if (!kmzx.Model.Site.Geometries.ContainsKey(s.ClassName))
                        {
                            readWarnings.AppendFormat("\r\nMDMD00: Suite type {0} has no geometry list in model.",
                                s.ClassName);
                        }
                        else
                        {
                            Geometry[] gl = kmzx.Model.Site.Geometries[s.ClassName];
                            foreach (Geometry geom in gl)
                            {
                                // This is a known problem.
                                // Model may have a geometry node in non-lines format.
                                if ((null == geom.Points) || (null == geom.Lines))
                                {
                                    readWarnings.AppendFormat("\r\nMDER00: Geometry ID={0} uses unknown format. Points and lines are not read.",
                                        geom.Id);
                                }
                            }
                        }                        
                    }
                }
            }

            // Generate test preview of parsed model
            //
            string kmlFile = Path.Combine(Path.GetDirectoryName(_modelFileName), "wireframe-test-output.kml");
            try
            {
                TestKmlWriter.GenerateCoordinateKml(kmzx, 0.0, kmlFile);
                readWarnings.AppendFormat("\r\n\r\nCreated test preview at {0}", kmlFile);
            }
            catch (Exception e)
            {
                File.Delete(kmlFile);
                readWarnings.AppendFormat("\r\n{0}\r\n{1}", e.Message, e.StackTrace);
            }

            return readWarnings.ToString();
        }

        public Form1()
        {
            InitializeComponent();

            Text = Text + " - v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            toolTip1.SetToolTip(btnGuessSuiteTypeInfo, "Try guessing Suite Type Info file");
            toolTip1.SetToolTip(btnGuessFloorPlanFolder, "Try guessing Fllor Plan folder");
        }

        private void onNewModelFileName(string filename)
        {
            if (Path.Equals(_modelFileName, filename)) return;

            _modelFileName = filename;
            lblModelPath.Text = _modelFileName;

            if (null == _stiFileName) guessSuiteTypeInfoFileName();
            else btnGuessSuiteTypeInfo.Enabled = true;

            if (null == _floorPlanPath) guessFloorPlanPath();
            else btnGuessFloorPlanFolder.Enabled = true;

            checkEnableTestButton();
        }

        private void guessFloorPlanPath()
        {
            string test;

            test = Path.Combine(Path.GetDirectoryName(_modelFileName), "SuitesWeb\\images");
            if (Directory.Exists(test))
            {
                _floorPlanPath = test;
                lblFloorPlansPath.Text = _floorPlanPath;
            }
            else
            {
                test = Path.Combine(Path.GetDirectoryName(_modelFileName), "SuitesWeb");
                if (Directory.Exists(test))
                {
                    _floorPlanPath = test;
                    lblFloorPlansPath.Text = _floorPlanPath;
                }
                else
                {
                    test = Path.Combine(Path.GetDirectoryName(_modelFileName), "Site");
                    if (Directory.Exists(test))
                    {
                        _floorPlanPath = test;
                        lblFloorPlansPath.Text = _floorPlanPath;
                    }
                    else
                    {
                        test = Path.GetDirectoryName(_modelFileName);
                        _floorPlanPath = test;
                        lblFloorPlansPath.Text = _floorPlanPath;
                    }
                }
            }

            btnGuessFloorPlanFolder.Enabled = false;
        }

        private void guessSuiteTypeInfoFileName()
        {
            string test;

            test = Path.Combine(
                Path.GetDirectoryName(_modelFileName),
                Path.GetFileNameWithoutExtension(_modelFileName))
                + ".csv";
            if (File.Exists(test))
            {
                _stiFileName = test;
                lblSuiteTypeInfoPath.Text = _stiFileName;
            }
            else
            {
                test = Path.Combine(
                    Path.GetDirectoryName(_modelFileName),
                    "sti.csv");
                if (File.Exists(test))
                {
                    _stiFileName = test;
                    lblSuiteTypeInfoPath.Text = _stiFileName;
                }
                else
                {
                    string[] candidates = Directory.GetFiles(
                        Path.GetDirectoryName(_modelFileName), "*.csv", SearchOption.TopDirectoryOnly);
                    if (1 == candidates.Length)
                    {
                        _stiFileName = candidates[0];
                        lblSuiteTypeInfoPath.Text = _stiFileName;
                    }
                }
            }

            btnGuessSuiteTypeInfo.Enabled = false;
        }

        private void onNewSuiteTypeInfoFileName(string filename)
        {
            if (Path.Equals(_stiFileName, filename)) return;

            _stiFileName = filename;
            lblSuiteTypeInfoPath.Text = _stiFileName;

            checkEnableTestButton();
        }

        private void onNewFloorPlanPath(string pathname)
        {
            if (Path.Equals(_floorPlanPath, pathname)) return;

            _floorPlanPath = pathname;
            lblFloorPlansPath.Text = _floorPlanPath;

            checkEnableTestButton();
        }

        private void checkEnableTestButton()
        {
            btnTest.Enabled = (_modelFileName != null) && (_stiFileName != null) && (_floorPlanPath != null);
        }

        private void btnBrowseModel_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.DefaultExt = "kmz";
            ofd.Filter = "Model files (*.kmz)|*.kmz|All Files (*.*)|*.*";
            ofd.Multiselect = false;
            //ofd.InitialDirectory = null;
            ofd.Title = "Select model file";

            if (DialogResult.OK == ofd.ShowDialog()) onNewModelFileName(ofd.FileName);
        }

        private void btnBrowseSuiteTypeInfo_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.DefaultExt = "csv";
            ofd.Filter = "Suite type info files (*.csv)|*.csv|All Files (*.*)|*.*";
            ofd.Multiselect = false;
            //ofd.InitialDirectory = null;
            ofd.Title = "Select suite type info file";

            if (DialogResult.OK == ofd.ShowDialog()) onNewSuiteTypeInfoFileName(ofd.FileName);
        }

        private void btnBrowseFloorPlanFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select floor plan folder";
            //fbd.SelectedPath = "";
            fbd.ShowNewFolderButton = false;

            if (DialogResult.OK == fbd.ShowDialog()) onNewFloorPlanPath(fbd.SelectedPath);
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            Cursor saved = Cursor.Current;

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Application.DoEvents();

                string result = doTest();

                tbResults.Text = result.Length > 0 ? result : "All OK!";
            }
            catch (Exception ex)
            {
                tbResults.Text = ex.Message + "\r\n" + ex.StackTrace;
            }
            finally
            {
                tbResults.Enabled = true;
                Cursor.Current = saved;
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] myFiles = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (myFiles != null)
                {
                    foreach (string file in myFiles)
                    {
                        if (File.GetAttributes(file).HasFlag(FileAttributes.Directory))
                        {
                            onNewFloorPlanPath(file);
                        }
                        else
                        {
                            string ext = Path.GetExtension(file).ToLowerInvariant();
                            if (ext.Equals(".kmz")) onNewModelFileName(file);
                            else if (ext.Equals(".csv")) onNewSuiteTypeInfoFileName(file);
                        }
                    }
                }
            }
        }

        private void btnGuessSuiteTypeInfo_Click(object sender, EventArgs e)
        {
            guessSuiteTypeInfoFileName();
            checkEnableTestButton();
        }

        private void btnGuessFloorPlanFolder_Click(object sender, EventArgs e)
        {
            guessFloorPlanPath();
            checkEnableTestButton();
        }
    }

    internal class TestKmlWriter
    {
        public static void GenerateCoordinateKml(Vre.Server.Model.Kmz.Kmz readModel, double altAdj,
            string path)
        {
            using (FileStream file = File.Create(path))
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
