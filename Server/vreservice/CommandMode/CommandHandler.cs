
namespace Vre.Server.Command
{
    public class CommandHandler
    {
        public static bool HandleCommand(string[] args)
        {
            bool result = false;
            Parameters par = new Parameters(args);

            if (par.ContainsParameter("importmodel"))
            {
                string infoModelFileName = par.GetOption("infomodel");
                string estateDeveloper = par.GetOption("ed");

                if ((null == infoModelFileName) || (null == estateDeveloper))
                {
                    // TODO: Print usage
                }
                else
                {
                    ModelImport.ImportModel(estateDeveloper, infoModelFileName, par);
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