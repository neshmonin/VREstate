﻿using System;
using System.Collections.Generic;
using Vre.Server.BusinessLogic;

namespace Vre.Client.CommandLine
{
    internal class CmdSwitchEd : CommandParserBase
    {
        public CmdSwitchEd()
        {
            Name = "switch-ed";
            Description = "-superadmin only";
        }

        public override void ShowHelp()
        {
            Console.WriteLine("Sets or changes current estate developer for superadmin.");
            Console.WriteLine("Usage: switch-ed <estate developer id>");
        }

        public override void Parse(ServerProxy proxy, List<string> elements)
        {
            if (elements.Count != 1)
            {
                ShowHelp();
            }
            else
            {
                if (Program.UserRole != User.Role.SuperAdmin)
                    throw new InvalidOperationException("This command is available to superadmins only.");

                Program.EstateDeveloperId = int.Parse(elements[0]);
            }
        }
    }
}