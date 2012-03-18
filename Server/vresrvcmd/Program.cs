using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using Vre.Server;
using Vre.Server.BusinessLogic;
using System.Collections.Generic;

namespace Vre.Client.CommandLine
{
    class Program
    {
        private static Dictionary<string, CommandParserBase> buildCommandList()
        {
            Dictionary<string, CommandParserBase> result = new Dictionary<string, CommandParserBase>();

            addCommand(ref result, new CmdLogin());
            addCommand(ref result, new CmdList());
            addCommand(ref result, new CmdSwitchEd());
            addCommand(ref result, new CmdChPwd());

            return result;
        }

        internal static string EstateDeveloperId = string.Empty;
        internal static User.Role UserRole = User.Role.Buyer;

        static void Main(string[] args)
        {
            Console.WriteLine(VersionGen.ProductName + " console v" + VersionGen.AssemblyVersionString);
            Console.WriteLine(VersionGen.CopyrightString);

            if (1 != args.Length)
            {
                Console.WriteLine("Usage: {0} <server end point in full URI format>", 
                    Path.GetFileName(Assembly.GetExecutingAssembly().GetName().CodeBase));
                return;
            }

            try
            {
                Dictionary<string, CommandParserBase> rootCommands = buildCommandList();

                string uriBase = args[0];
                if (!uriBase.EndsWith("/")) uriBase += "/";

                using (ServerProxy proxy = new ServerProxy(uriBase, 15, null))
                {
                    Console.WriteLine("Connecting to {0}...", uriBase);
                    if (!proxy.Test())
                    {
                        Console.WriteLine("Server is not reachable.");
                        return;
                    }

                    Console.WriteLine("Reached.  Enter '?' for help.");

                    while (proxy.Online || string.IsNullOrEmpty(proxy.LoginId))
                    {
                        Console.Write("{0}@{1}>", proxy.LoginId, EstateDeveloperId);
                        List<string> cmdElements = preParseCommand(Console.ReadLine());

                        if (cmdElements.Count > 0)
                        {
                            string command = cmdElements[0];
                            cmdElements.RemoveAt(0);

                            if (command.Equals("help") || command.Equals("?"))
                            {
                                showHelp(rootCommands, cmdElements);
                            }
                            else if (command.Equals("exit")) break;
                            else
                            {
                                CommandParserBase cmd;
                                if (rootCommands.TryGetValue(command, out cmd))
                                {
                                    try
                                    {
                                        cmd.Parse(proxy, cmdElements);
                                    }
                                    catch (ArgumentException ex)
                                    {
                                        Console.WriteLine("Syntax error: {0}", ex.Message);
                                        Console.WriteLine("Type '{0}' with no parameters or '? {0}' to get help", command);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("ERROR: {0}", ex.Message);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Unknown command; type '?' for a list of commands.");
                                }
                            }
                        }
                    }

                    if (proxy.Online || string.IsNullOrEmpty(proxy.LoginId)) Console.WriteLine("Bye.");
                    else Console.WriteLine("Server connection lost: {0}", proxy.SessionLooseDescription);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected error: {0}\r\n{1}", ex.Message, ex.StackTrace);
            }
        }

        private static List<string> preParseCommand(string command)
        {
            List<string> result = new List<string>();

            foreach (string element in command.Split(' '))
                if (element.Length > 0) result.Add(element);            

            return result;
        }

        private static void addCommand(ref Dictionary<string, CommandParserBase> store, CommandParserBase cmd)
        {
            store.Add(cmd.Name, cmd);
        }

        private static void showHelp(Dictionary<string, CommandParserBase> store, List<string> cmdElements)
        {
            if (1 == cmdElements.Count)
            {
                string name = cmdElements[0];

                if (name.Equals("exit"))
                {
                    Console.WriteLine("Closes console and returns back to OS.  No parameters expected.");
                    return;
                }
                else if (name.Equals("help") || name.Equals("?"))
                {
                    Console.WriteLine("Please type 'exit'");
                    return;
                }

                CommandParserBase command;
                if (store.TryGetValue(name, out command))
                {
                    command.ShowHelp();
                    return;
                }
            }

            Console.WriteLine("Currently available commands (enter command with no parameters to get command help):");
            Console.WriteLine("  help or ?");
            Console.WriteLine("  help <command name> -help on specific command");
            Console.WriteLine("  exit");
            foreach (CommandParserBase cmd in store.Values)
                Console.WriteLine("  {0} {1}", cmd.Name, cmd.Description);
            Console.WriteLine("Commands and parameters ARE case-sensitive.");

            //Console.WriteLine("create");
            //Console.WriteLine("update");
        }

        #region create
        private static void showCreateHelp()
        {
            Console.WriteLine("Usage: create <entity> [<parameters>]");
            Console.WriteLine("Where entity can be:");
            Console.WriteLine("  user -parameters are:");
            Console.WriteLine("    login -unique new user login; required.");
            Console.WriteLine("    ed=<estate developer id> -required for any non-admin users.");
            Console.WriteLine("    role={admin|developer|subcont|sales|buyer} -required.");
        }

        #endregion
    }
}
