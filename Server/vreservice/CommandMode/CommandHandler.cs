using System.Collections.Generic;

namespace Vre.Server.Command
{
    public class CommandHandler
    {
        private static Dictionary<string, ICommand> _commands;

        static CommandHandler()
        {
            _commands = new Dictionary<string, ICommand>();
            addCommand(new ModelImport());
            addCommand(new TaskRunner());
        }

        private static void addCommand(ICommand cmd) { _commands.Add(cmd.Name, cmd); }

        public static bool HandleCommand(string[] args)
        {
            bool result = false;
            Parameters par = new Parameters(args);

            foreach (string cmdKey in _commands.Keys)
            {
                if (par.ContainsParameter(cmdKey))
                {
                    par.RemoveKey(cmdKey);
                    _commands[cmdKey].Execute(par);
                    result = true;
                }
            }

            return result;
        }

        public static bool str2bool(string param, bool defaultValue)
        {
            bool result;
            string lc = null;
            if (param != null) lc = param.ToLower();
            result = (lc != null) ?
                (lc.Equals("yes") || lc.Equals("true")
                || lc.Equals("y") || lc.Equals("1")) : defaultValue;
            return result;
        }
    }
}