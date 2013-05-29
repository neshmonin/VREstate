using System;
using System.Text;
using Vre.Server.Command;
using Vre.Server.RemoteService;

namespace Vre.Server.Task
{
    internal class RetroImportViewOrderTargets : BaseTask
    {
        public override string Name { get { return "RetroImportViewOrderTargets"; } }

        public override void Execute(Parameters param)
        {
			int id;

			if (!int.TryParse(param.GetOption("buildingId"), out id))
				throw new ArgumentException("Must specify integer building ID");

			StringBuilder report = new StringBuilder();
			using (var session = ClientSession.MakeSystemSession())
			{
				session.Resume();
				ModelImport.TryImportExistingListings(session, id, report);
			}

			ServiceInstances.Logger.Info(report.ToString());
        }
    }
}