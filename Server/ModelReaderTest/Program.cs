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
            string modelFileName;
            if (args.Length > 1) return;
            if (args.Length < 1) 
                modelFileName = @"C:\Documents and Settings\port443\My Documents\My Dropbox\AndrewShare\Eden Park Towers (Phase II) - Read Only.kmz";
                //modelFileName = @"C:\Documents and Settings\port443\My Documents\My Dropbox\AndrewShare\SuperServer\Times Group Corporation\Eden_Park_Towers_Phase_II\Model.kmz";
            else 
                modelFileName = args[0];

            VrEstate.Site siteData;

            Console.Write("Reading {0}...", modelFileName);

            try
            {
                using (VrEstate.Kmz kmz = VrEstate.Kmz.Open(modelFileName, System.IO.FileAccess.Read))
                {
                    VrEstate.Model.Setup(kmz.GetKmlDoc());
                    siteData = new VrEstate.Site(kmz.GetColladaDoc());
                }

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
    }
}
