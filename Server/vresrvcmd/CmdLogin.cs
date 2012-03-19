using System;
using System.Collections.Generic;
using System.Net;
using Vre.Server.BusinessLogic;

namespace Vre.Client.CommandLine
{
    internal class CmdLogin : CommandParserBase
    {
        public CmdLogin()
        {
            Name = "login";
            Description = string.Empty;
        }

        public override void ShowHelp()
        {
            Console.WriteLine("Usage: login <login>");
            Console.WriteLine("Password is entered interactively.");
        }

        public override void Parse(ServerProxy proxy, List<string> elements)
        {
            if (elements.Count < 1)
            {
                ShowHelp();
            }
            else if (elements.Count > 1)
            {
                throw new ArgumentException("Unknown parameters");
            }
            else
            {
                string login = elements[0];
                string pwd = readPassword("Enter password: ");

                if (proxy.Login(login, pwd))
                {
                    ServerResponse resp = proxy.MakeRestRequest(ServerProxy.RequestType.Get, "user/0", null, null);
                    if (HttpStatusCode.OK == resp.ResponseCode)
                    {
                        ClientData userInfo = resp.Data;
                        Program.EstateDeveloperId = userInfo.GetProperty("estateDeveloperId", -1);
                        Program.UserRole = userInfo.GetProperty("role", User.Role.Buyer);
                    }
                }
                else
                {
                    Console.WriteLine("Unknown login or bad password.");
                }
            }
        }
    }
}