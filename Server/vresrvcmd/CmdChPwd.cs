using System;
using System.Collections.Generic;
using System.Net;

namespace Vre.Client.CommandLine
{
    internal class CmdChPwd : CommandParserBase
    {
        public CmdChPwd()
        {
            Name = "chpwd";
            Description = string.Empty;
        }

        public override void ShowHelp()
        {
            Console.WriteLine("Usage: chpwd [<login>]");
            Console.WriteLine("Current and new password is entered interactively.");
        }

        public override void Parse(ServerProxy proxy, List<string> elements)
        {
            if (elements.Count > 1)
            {
                throw new ArgumentException("Unknown parameters");
            }
            else
            {
                string login;
                if (elements.Count > 0) login = elements[0];
                else login = proxy.LoginId;

                string cpwd = readPassword("Enter current password for '" + login + "' (empty to abort): ");
                if (0 == cpwd.Length) return;

                string npwd0 = readPassword("Enter new password: ");
                string npwd1 = readPassword("Repeat new password: ");
                if (!npwd0.Equals(npwd1)) throw new ArgumentException("New password entries do not match");

                ServerResponse resp = proxy.MakeProgramCall(string.Format("q=chpwd&uid={0}&pwd={1}&npwd={2}",
                    login, cpwd, npwd0));

                if (HttpStatusCode.OK == resp.ResponseCode)
                {
                    Console.WriteLine("Password for '{0}' is updated.", login);
                }
                else
                {
                    Console.WriteLine("Password was not updated: {0}", resp.ResponseCodeDescription);
                }
            }
        }
    }
}