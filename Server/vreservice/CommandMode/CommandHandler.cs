
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
                string modelFileName = par.GetOption("model");
                string extraSuiteInfoFileName = par.GetOption("sti");
                string estateDeveloper = par.GetOption("ed");
                string constructionSite = par.GetOption("site");
                string strDryRun = par.GetOption("dryrun");

                if (strDryRun != null) strDryRun = strDryRun.ToLower();
                bool dryRun = (strDryRun != null) ?
                    (strDryRun.Equals("yes") || strDryRun.Equals("true")
                    || strDryRun.Equals("y") || strDryRun.Equals("1")) : false;

                if ((null == modelFileName) || (null == estateDeveloper))
                {
                    // TODO: Print usage
                }
                else
                {
                    ModelImport.ImportModel(estateDeveloper, constructionSite, modelFileName, extraSuiteInfoFileName, dryRun);
                    result = true;
                }
            }

            return result;
        }
    }
}