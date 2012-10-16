
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
                string displayModelFileName = par.GetOption("displaymodel");
                string extraSuiteInfoFileName = par.GetOption("sti");
                string estateDeveloper = par.GetOption("ed");
                string constructionSite = par.GetOption("site");
                string building = par.GetOption("building");
                string strDryRun = par.GetOption("dryrun");
                string strAsBuilding = par.GetOption("asbuilding");

                bool dryRun = str2bool(strDryRun, true);
                bool asBuilding = str2bool(strAsBuilding, false);

                if ((null == infoModelFileName) || (null == estateDeveloper))
                {
                    // TODO: Print usage
                }
                else
                {
                    ModelImport.ImportModel(estateDeveloper, constructionSite, building,
                        infoModelFileName, displayModelFileName, extraSuiteInfoFileName, !asBuilding, dryRun);
                    result = true;
                }
            }

            return result;
        }

        private static bool str2bool(string param, bool defaultValue)
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