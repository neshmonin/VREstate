using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.Command;
using Vre.Server.Dao;
using Vre.Server.FileStorage;
using Vre.Server.RemoteService;

namespace Vre.Server.Task
{
	internal abstract class ManagedStorageTaskBase : BaseTask
	{
		public override void Execute(Parameters param)
		{
			var report = new StringBuilder();
			var unusedFileList = new List<string>();

			using (var session = ClientSession.MakeSystemSession())
			{
				DatabaseSettingsDao.VerifyDatabase();
				session.Resume();

				processStorage(session.DbSession,
					FileStorageItem.LocationType.Public, ServiceInstances.FileStorageManager,
					Configuration.PublicFileStore.RootPath.Value, new [] { "models", "user" },
					getAllPublicFilesUsedByDb(session.DbSession),
					ref report, ref unusedFileList);

				if (unusedFileList.Count > 0)
				{
					var fn = Path.Combine(Configuration.PublicFileStore.RootPath.Value, string.Format("@unused-files-{0:yyyyMMddHHmm}.txt", DateTime.Now));
					report.AppendFormat("Found {0} unused files; saved in {1}\r\n", unusedFileList.Count, fn);
					dumpFileList(fn, "MOVE ", DateTime.Now.ToString("yyyyMMdd"), true, unusedFileList);
					unusedFileList.Clear();
				}

				processStorage(session.DbSession,
					FileStorageItem.LocationType.Internal, ServiceInstances.InternalFileStorageManager,
					Configuration.InternalFileStore.RootPath.Value, new[] { "wireframes" },
					getAllInternalFilesUsedByDb(session.DbSession),
					ref report, ref unusedFileList);

				if (unusedFileList.Count > 0)
				{
					var fn = Path.Combine(Configuration.InternalFileStore.RootPath.Value, string.Format("@unused-files-{0:yyyyMMddHHmm}.txt", DateTime.Now));
					report.AppendFormat("Found {0} unused files; saved in {1}\r\n", unusedFileList.Count, fn);
					dumpFileList(fn, "MOVE ", DateTime.Now.ToString("yyyyMMdd"), true, unusedFileList);
					unusedFileList.Clear();
				}
			}

			ServiceInstances.Logger.Info(report.ToString());
		}

		protected abstract void processStorage(ISession dbSession,
			FileStorageItem.LocationType locationType,
			IFileStorageManager manager,
			string storageRoot, IEnumerable<string> namespaceHints,
			IEnumerable<string> dbFileList,
			ref StringBuilder report, ref List<string> unusedFileList);

		protected static bool transactOp(ISession dbSession, Func<ISession, FileStorageItemDao, bool> op)
		{
			bool modified;
			using (var tran = NHibernateHelper.OpenNonNestedTransaction(dbSession))
			{
				using (var tdao = new FileStorageItemDao(dbSession))
				{
					modified = op(dbSession, tdao);
				}
				if (modified) tran.Commit();
			}
			return modified;
		}

		protected static HashAlgorithm GetHasher() { return MD5.Create(); }

		private static IEnumerable<string> getAllPublicFilesUsedByDb(
			ISession dbSession)
		{
			using (var dao = new StructureDao(dbSession))
			{
				foreach (var s in dao.GetAll())
				{
					if (!string.IsNullOrEmpty(s.DisplayModelUrl)) yield return s.DisplayModelUrl;
				}
			}
			using (var dao = new SiteDao(dbSession))
			{
				foreach (var s in dao.GetAll())
				{
					if (!string.IsNullOrEmpty(s.DisplayModelUrl)) yield return s.DisplayModelUrl;
					if (!string.IsNullOrEmpty(s.OverlayModelUrl)) yield return s.OverlayModelUrl;
					if (!string.IsNullOrEmpty(s.PoiModelUrl)) yield return s.PoiModelUrl;
					if (!string.IsNullOrEmpty(s.BubbleKioskTemplateUrl)) yield return s.BubbleKioskTemplateUrl;
					if (!string.IsNullOrEmpty(s.BubbleWebTemplateUrl)) yield return s.BubbleWebTemplateUrl;
				}
			}
			using (var dao = new BuildingDao(dbSession))
			{
				foreach (var b in dao.GetAll())
				{
					if (!string.IsNullOrEmpty(b.DisplayModelUrl)) yield return b.DisplayModelUrl;
					if (!string.IsNullOrEmpty(b.OverlayModelUrl)) yield return b.OverlayModelUrl;
					//if (!string.IsNullOrEmpty(b.PoiModelUrl)) yield return b.PoiModelUrl;
					if (!string.IsNullOrEmpty(b.BubbleKioskTemplateUrl)) yield return b.BubbleKioskTemplateUrl;
					if (!string.IsNullOrEmpty(b.BubbleWebTemplateUrl)) yield return b.BubbleWebTemplateUrl;
				}
			}
			using (var dao = new SuiteTypeDao(dbSession))
			{
				foreach (var st in dao.GetAll())
				{
					if (!string.IsNullOrEmpty(st.FloorPlanUrl)) yield return st.FloorPlanUrl;
				}
			}
			using (var dao = new UserDao(dbSession))
			{
				foreach (var st in dao.GetAll())
				{
					if (!string.IsNullOrEmpty(st.PhotoUrl)) yield return st.PhotoUrl;
				}
			}
		}

		private static IEnumerable<string> getAllInternalFilesUsedByDb(
			ISession dbSession)
		{
			using (var dao = new SiteDao(dbSession))
			{
				foreach (var s in dao.GetAll())
				{
					if (!string.IsNullOrEmpty(s.WireframeLocation)) yield return s.WireframeLocation;
				}
			}
			using (var dao = new BuildingDao(dbSession))
			{
				foreach (var b in dao.GetAll())
				{
					if (!string.IsNullOrEmpty(b.WireframeLocation)) yield return b.WireframeLocation;
				}
			}
		}

		private static void dumpFileList(string outputPath, string prefix, string postfix, bool quote, IEnumerable<string> list)
		{
			using (var fs = File.CreateText(outputPath))
			{
				foreach (var i in list)
				{
					if (string.IsNullOrEmpty(prefix))
					{
						fs.WriteLine(i);
					}
					else if (string.IsNullOrEmpty(postfix))
					{
						if (quote) fs.WriteLine("{0} \"{1}\"", prefix, i);
						else fs.WriteLine("{0} {1}", prefix, i);
					}
					else
					{
						if (quote) fs.WriteLine("{0} \"{1}\" {2}", prefix, i, postfix);
						else fs.WriteLine("{0} {1} {2}", prefix, i, postfix);
					}
				}
			}
		}
	}
}