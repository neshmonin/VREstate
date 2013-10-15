using System.Text;
using Vre.Server.Command;
using Vre.Server.Dao;
using Vre.Server.RemoteService;

namespace Vre.Server.Task
{
    internal class RetroImportViewOrderTargets : BaseTask
    {
        public override string Name { get { return "RetroImportViewOrderTargets"; } }

        public override void Execute(Parameters param)
        {
			int id;

			StringBuilder report = new StringBuilder();

			using (var session = ClientSession.MakeSystemSession())
			{
				DatabaseSettingsDao.VerifyDatabase();
				session.Resume();
				if (!int.TryParse(param.GetOption("buildingId"), out id))
					ModelImport.TryImportExistingListings(session, report);
				else
					ModelImport.TryImportExistingListings(session, id, report);
			}

			ServiceInstances.Logger.Info(report.ToString());
        }
    }
}