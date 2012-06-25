using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Vre.Server.BusinessLogic;

namespace Vre.Client.CommandLine
{
    internal class CmdList : CommandParserBase
    {
        public CmdList()
        {
            Name = "list";
            Description = string.Empty;
        }

        public override void ShowHelp()
        {
            Console.WriteLine("Usage: list [all] <entity> [<parameters>]");
            Console.WriteLine("Where entity can be:");
            Console.WriteLine("  developer -no parameters expected; superadmin only.");
            Console.WriteLine("  site -limited to estate developer scope; parameters are:");
            Console.WriteLine("    ed=<estate developer id> -optional; superadmin only;");
            Console.WriteLine("        set or override current value.");
            Console.WriteLine("  building -parameters are:");
            Console.WriteLine("    site=<construction site id> -required.");
            Console.WriteLine("  user -limited to estate developer scope; parameters are:");
            Console.WriteLine("    role={superadmin|developeradmin|subcontractor|salesperson|buyer|sellingagent|kiosk}");
            Console.WriteLine("        -optional, defaults to buyer.");
            Console.WriteLine("    ed=<estate developer id> -optional; superadmin only;");
            Console.WriteLine("        set or override current value.");
            Console.WriteLine("    name=<full or partial name> -optional; filter by name.");
            Console.WriteLine("Specifying 'all' parameter shall include deleted objects.");
        }

        public override void Parse(ServerProxy proxy, List<string> elements)
        {
            bool listAll = false;
            Dictionary<string, string> options;

            if (0 == elements.Count)
            {
                ShowHelp();
                return;
            }

            // object name
            //
            string obj = elements[0];
            elements.RemoveAt(0);

            options = parseOptions(elements);

            if (options.ContainsKey("all")) listAll = true;

            if (obj.Equals("developer"))
                listDevelopers(proxy, listAll, options);
            else if (obj.Equals("site"))
                listSites(proxy, listAll, options);
            else if (obj.Equals("building"))
                listBuildings(proxy, listAll, options);
            else if (obj.Equals("user"))
                listUsers(proxy, listAll, options);
            else
                throw new ArgumentException("Unknown object type to list.");
        }

        private static void listDevelopers(ServerProxy proxy, bool all, Dictionary<string, string> options)
        {
            if (Program.UserRole != User.Role.SuperAdmin)
                throw new InvalidOperationException("This command is available to superadmins only.");

            ServerResponse resp = proxy.MakeRestRequest(ServerProxy.RequestType.Get, "ed", 
                "withdeleted=" + (all ? "true" : "false"), null);
            if (HttpStatusCode.OK == resp.ResponseCode)
            {
                Console.WriteLine("ID   Del  Cfg  Name");
                Console.WriteLine("----------------------------------------");
                foreach (ClientData info in resp.Data.GetNextLevelDataArray("developers"))
                {
                    Console.WriteLine("{0}   {1}   {2}   {3}",
                        info.GetProperty("id", -1),
                        info.GetProperty("deleted", false),
                        info.GetProperty("configuration", 0),
                        info.GetProperty("name", "?"));
                }
            }
            else
            {
                throw new Exception("Server error: " + resp.ResponseCodeDescription);
            }
        }

        private static void listSites(ServerProxy proxy, bool all, Dictionary<string, string> options)
        {
            StringBuilder query = new StringBuilder();
            string paramValue;

            query.AppendFormat("withdeleted={0}", (all ? "true" : "false"));

            if (options.TryGetValue("ed", out paramValue))
            {
                if (Program.UserRole != User.Role.SuperAdmin)
                    throw new InvalidOperationException("Specifying estate developer ID is available to superadmins only.");
            }
            else
            {
                paramValue = Program.EstateDeveloperId.ToString();
            }
            if (string.IsNullOrEmpty(paramValue))
                throw new ArgumentException("Estate developer ID must be specified.");
            query.AppendFormat("&ed={0}", paramValue);
            
            ServerResponse resp = proxy.MakeRestRequest(ServerProxy.RequestType.Get, "site", query.ToString(), null);
            if (HttpStatusCode.OK == resp.ResponseCode)
            {
                Console.WriteLine("ID   Del  Name");
                Console.WriteLine("----------------------------------------");
                foreach (ClientData info in resp.Data.GetNextLevelDataArray("sites"))
                {
                    Console.WriteLine("{0}   {1}   {2}",
                        info.GetProperty("id", -1),
                        info.GetProperty("deleted", false),
                        info.GetProperty("name", "?"));
                }
            }
            else
            {
                throw new Exception("Server error: " + resp.ResponseCodeDescription);
            }
        }

        private static void listBuildings(ServerProxy proxy, bool all, Dictionary<string, string> options)
        {
            StringBuilder query = new StringBuilder();
            string paramValue;

            query.AppendFormat("withdeleted={0}", (all ? "true" : "false"));

            if (!options.TryGetValue("site", out paramValue))
            {
                throw new ArgumentException("Construction site ID must be specified.");
            }
            query.AppendFormat("&site={0}", paramValue);

            ServerResponse resp = proxy.MakeRestRequest(ServerProxy.RequestType.Get, "building", query.ToString(), null);
            if (HttpStatusCode.OK == resp.ResponseCode)
            {
                Console.WriteLine("ID   Del  Name     Status       OpeningDate       Altitude adj");
                Console.WriteLine("---------------------------------------------------------------");
                foreach (ClientData info in resp.Data.GetNextLevelDataArray("buildings"))
                {
                    Console.WriteLine("{0}   {1}   {2}   {3}   {4}   {5}",
                        info.GetProperty("id", -1),
                        info.GetProperty("deleted", false),
                        info.GetProperty("name", "?"),
                        info.GetProperty("status", Building.BuildingStatus.InProject),
                        info.GetProperty("openingDate", DateTime.MinValue),
                        info.GetProperty("altitudeAdjustment", 0.0));
                }
            }
            else
            {
                throw new Exception("Server error: " + resp.ResponseCodeDescription);
            }
        }

        private static void listUsers(ServerProxy proxy, bool all, Dictionary<string, string> options)
        {
            StringBuilder query = new StringBuilder();
            string paramValue;
            bool saList = false;

            query.AppendFormat("withdeleted={0}", (all ? "true" : "false"));

            if (options.TryGetValue("role", out paramValue))
            {
                query.AppendFormat("&role={0}", paramValue);
                if (paramValue.Equals("superadmin")) saList = true;
            }
            
            if (options.TryGetValue("name", out paramValue))
                query.AppendFormat("&nameFilter={0}", paramValue);

            if (options.TryGetValue("ed", out paramValue))
            {
                if (Program.UserRole != User.Role.SuperAdmin)
                    throw new InvalidOperationException("This command is available to superadmins only.");
                if (saList)
                    throw new ArgumentException("Superadmins list cannot have estate developer id.");
            }
            else
            {
                paramValue = Program.EstateDeveloperId.ToString();
            }
            if (saList)
            {
                query.Append("&ed=-1");
            }
            else
            {
                if (string.IsNullOrEmpty(paramValue))
                    throw new ArgumentException("Estate developer ID must be specified.");
                query.AppendFormat("&ed={0}", paramValue);
            }

            ServerResponse resp = proxy.MakeRestRequest(ServerProxy.RequestType.Get, "user", query.ToString(), null);
            if (HttpStatusCode.OK == resp.ResponseCode)
            {
                Console.WriteLine("ID   Del   Type  Login  Name");
                Console.WriteLine("----------------------------------------");
                foreach (ClientData info in resp.Data.GetNextLevelDataArray("users"))
                {
                    Console.WriteLine("{0}  {1}  {2}  {3}  {4}",
                        info.GetProperty("id", -1),
                        info.GetProperty("deleted", false),
                        info.GetProperty("loginType", LoginType.Plain),
                        info.GetProperty("login", "?"),
                        retrieveUsername(info));
                }
            }
            else
            {
                throw new Exception("Server error: " + resp.ResponseCodeDescription);
            }
        }

        private static string retrieveUsername(ClientData userInfo)
        {
            ClientData pi = userInfo.GetNextLevelDataItem("personalInfo");
            string first = pi.GetProperty("firstName", string.Empty);
            if (first.Length > 0)
                return string.Format("{0}{1} {2}",
                    pi.GetProperty("personalTitle", string.Empty),
                    first,
                    pi.GetProperty("lastName", string.Empty));
            else
                return "?";
        }
    }
}