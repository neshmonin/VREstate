using System;
using System.Collections.Generic;
using System.Net;
using Vre.Server.BusinessLogic;

namespace Vre.Client.CommandLine
{
    internal class CmdCreate : CommandParserBase
    {
        public CmdCreate()
        {
            Name = "create";
            Description = string.Empty;
        }

        public override void ShowHelp()
        {
            Console.WriteLine("Usage: create <entity> [<parameters>]");
            Console.WriteLine("Where entity can be:");
            Console.WriteLine("  user -parameters are:");
            Console.WriteLine("    login=<login> -unique new user login; required.");
            Console.WriteLine("    ed=<estate developer id> -required for any non-superadmin users.");
            Console.WriteLine("    role={superadmin|developeradmin|subcontractor|salesperson|buyer|sellingagent|kiosk} -required.");
            Console.WriteLine("Password is entered interactively.");
        }

        public override void Parse(ServerProxy proxy, List<string> elements)
        {
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

            // extra required/optional parameters' check
            //
            if (obj.Equals("user"))
                createUser(proxy, options);
            else
                throw new ArgumentException("Unknown object type to list.");
        }

        private static void createUser(ServerProxy proxy, Dictionary<string, string> options)
        {
            ClientData data = new ClientData();
            string paramValue;

            // role
            //
            User.Role role;
            if (options.TryGetValue("role", out paramValue))
            {
                if (paramValue.Equals("superadmin")) role = User.Role.SuperAdmin;
                else if (paramValue.Equals("developeradmin")) role = User.Role.DeveloperAdmin;
                else if (paramValue.Equals("subcontractor")) role = User.Role.Subcontractor;
                else if (paramValue.Equals("salesperson")) role = User.Role.SalesPerson;
                else if (paramValue.Equals("buyer")) role = User.Role.Buyer;
                else if (paramValue.Equals("sellingagent")) role = User.Role.SellingAgent;
                else if (paramValue.Equals("kiosk")) role = User.Role.Kiosk;
                else if (paramValue.Equals("brokerageadmin")) role = User.Role.BrokerageAdmin;
                else throw new ArgumentException("Role specified is not valid.");
                data.Add("role", role);
            }
            else
            {
                throw new ArgumentException("Must specify user role.");
            }

            // estate developer id
            //
            if (options.TryGetValue("ed", out paramValue))
            {
                if (Program.UserRole != User.Role.SuperAdmin)
                    throw new InvalidOperationException("Specifying estate developer ID is available to superadmins only.");
            }
            else
            {
                paramValue = Program.EstateDeveloperId.ToString();
            }
            if (User.IsEstateDeveloperTied(role))
            {
                if (string.IsNullOrEmpty(paramValue))
                    throw new ArgumentException("Estate developer ID must be specified.");
                int edId;
                if (int.TryParse(paramValue, out edId))
                    data.Add("ed", edId);
                else
                    throw new ArgumentException("The estate developer ID is not valid.");
            }

            // login
            //
            if (options.TryGetValue("login", out paramValue))
                data.Add("uid", paramValue);
            else
                throw new ArgumentException("Login must be specified.");

            data.Add("type", LoginType.Plain);

            string pwd0 = readPassword("Enter password: ");
            string pwd1 = readPassword("Repeat password: ");
            if (!pwd0.Equals(pwd1)) throw new ArgumentException("Password entries do not match");
            data.Add("pwd", pwd0);

            ServerResponse resp = proxy.MakeRestRequest(ServerProxy.RequestType.Insert, "user", null, data);
            if (HttpStatusCode.OK == resp.ResponseCode)
            {
                Console.WriteLine("User created successfully.");
            }
            else
            {
                Console.WriteLine(resp.ResponseCodeDescription);
            }
        }
    }
}