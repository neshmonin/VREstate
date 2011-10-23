using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VrEstate
{
    public class Developer
    {
        private List<Site> m_sites = new List<Site>();
        private string m_name;

        public string Name { get { return m_name; } }

        public List<Site> Sites { get { return m_sites; } }

        public Developer(string name)
        {
            string devPath;
            if (name.Contains('\\'))
            {
                m_name = name.Substring(name.LastIndexOf("\\") + 1);
                devPath = name;
            }
            else
            {
                m_name = name;
                devPath = Path.Combine(SuperServer.WebRootLocalPath, "SuperServer", m_name);
            }
            string[] dirs = null;
            try
            {
                dirs = Directory.GetDirectories(devPath);
            }
            catch (UnauthorizedAccessException UAEx)
            {
                Console.WriteLine(UAEx.Message);
            }
            catch (PathTooLongException PathEx)
            {
                Console.WriteLine(PathEx.Message);
            }

            if (dirs == null) return;

            Kmz.DeveloperName = m_name;
            foreach (string dir in dirs)
            {
                Kmz.SiteName = dir.Substring(dir.LastIndexOf("\\") + 1);
                using (Kmz kmz = Kmz.Open("Model.kmz", System.IO.FileAccess.Read))
                {
                    Model.Setup(kmz.GetKmlDoc());
                    Site site = new Site(kmz.GetColladaDoc());
                    site.DirName = Kmz.SiteName;
                    m_sites.Add(site);
                } 
            }
        }

        public bool SiteExists(Site site)
        {
            return m_sites.Exists(delegate(Site match)
            {
                return match.Name == site.Name;
            });
        }

        public Site Find(string name)
        {
            return m_sites.Find(delegate(Site match)
            {
                return match.Name == name;
            });
        }

        public bool AddSite(Site newSite)
        {
            if (SiteExists(newSite))
                return false;

            m_sites.Add(newSite);
            return true;
        }

        public bool RemoveSiteByName(string name)
        {
            Site oldSite = Find(name);
            if (oldSite == null)
                return false;

            return m_sites.Remove(oldSite);
        }

        public bool RemoveSite(Site site)
        {
            return m_sites.Remove(site);
        }

        public override string ToString()
        {
            return m_name;
        }
    }
}
