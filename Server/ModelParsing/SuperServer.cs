using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VrEstate
{
    public class SuperServer
    {
        private List<Developer> m_developers = new List<Developer>();
        private static string m_webRootLocalPath = @"C:\MyProjects\VirtualEstate\Trunk\VREstate\VrEstate\WebRoot";
        public static string WebRootLocalPath
        {
            get { return SuperServer.m_webRootLocalPath; }
            set { SuperServer.m_webRootLocalPath = value; }
        }

        public List<Developer> Developers
        {
            get { return m_developers; }
        }

        private SuperServer()
        {
            string superServer = Path.Combine(m_webRootLocalPath, "SuperServer");
            string[] dirs = null;
            try
            {
                dirs = Directory.GetDirectories(superServer);
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

            foreach (string dir in dirs)
            {
                Developer dev = new Developer(dir);
                m_developers.Add(dev);
            }
        }

        public static SuperServer Login()
        {
            SuperServer server = new SuperServer();
            return server;
        }
    }
}
