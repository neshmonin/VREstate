using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Text;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;
using Vre.Server.FileStorage;
using Vre.Server.Mls;
using Vre.Server.Model;
using Vre.Server.RemoteService;

namespace Vre.Server.Command
{
    internal class StructureImport : ICommand
    {
        private CsvSuiteTypeInfo _extraSuiteInfo;
        private StringBuilder _log;
        private Dictionary<string, SuiteType> _typeCache;
        private ClientSession _clientSession;
        private string _importPath;
        private List<string> _filesSaved = new List<string>();

        public string Name { get { return "importstructure"; } }

        public void Execute(Parameters param)
        {
            string displayModelFileName = param.GetOption("displaymodel");
            string structName = param.GetOption("structure");

            if (string.IsNullOrWhiteSpace(displayModelFileName)) throw new ArgumentException("Required parameter missing: displaymodel");
            if (string.IsNullOrWhiteSpace(structName)) throw new ArgumentException("Required parameter missing: structure");

            bool dryRun = CommandHandler.str2bool(param.GetOption("dryrun"), true);

            _log = new StringBuilder();
            string logFileName = Path.Combine(
                    Path.GetDirectoryName(displayModelFileName),
                    Path.GetFileNameWithoutExtension(displayModelFileName))
                + ".import.log.txt";
            FileStream logFile = File.Open(logFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);

            _log.AppendLine("=========================================================");
            _log.AppendLine(Environment.CommandLine);
            _log.AppendLine("---------------------------------------------------------");

            Exception importError = null;

            try
            {
                DatabaseSettingsDao.VerifyDatabase();

                doImport(structName, displayModelFileName, dryRun, param);
                //instance.generateSqlScript(estateDeveloperName, siteName, modelFileName, extraSuiteInfoFileName);
            }
            catch (Exception e)
            {
                importError = e;
                _log.AppendFormat("Error importing structure: {0}", Utilities.ExplodeException(e));
            }

            logFile.Seek(0, SeekOrigin.End);
            using (StreamWriter sw = new StreamWriter(logFile)) sw.Write(_log);

            if (importError != null) throw importError;
        }

        private void doImport(string structureName, 
            string displayModelFileName, 
            bool dryRun, Parameters extras)
        {
            _importPath = Path.GetDirectoryName(displayModelFileName);

            //_session = NHibernateHelper.GetSession();
            using (_clientSession = ClientSession.MakeSystemSession())//_session))
            {
                _clientSession.Resume();
				DatabaseSettingsDao.VerifyDatabase();
                //_clientSession.DbSession.FlushMode = FlushMode.Always;
                using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_clientSession.DbSession))
                {
					StringBuilder readWarnings = new StringBuilder();
					Vre.Server.Model.Kmz.Kmz kmz = new Vre.Server.Model.Kmz.Kmz(displayModelFileName, readWarnings);
										
					Structure structure;

                    using (var dao = new StructureDao(_clientSession.DbSession))
                        structure = dao.GetById(structureName);

                    if (null == structure)
                    {
						structure = new Structure(structureName);
                        _clientSession.DbSession.Save(structure);
                        _log.AppendFormat("Created new structure provided by command-line ID={0}, Name={1}\r\n", structure.AutoID, structure.Name);
                    }
                    else
                    {
						_log.AppendFormat("Using structure provided by command-line ID={0}, Name={1}\r\n", structure.AutoID, structure.Name);
                    }

					structure.AltitudeAdjustment = 0.0;
					structure.Location = kmz.Model.Site.Structures.First().LocationCart.AsGeoPoint();

					structure.DisplayModelUrl = storeModelFile(structure, structure.DisplayModelUrl, 
						displayModelFileName, "str");

					// set localized name: if provided - use it; else if exists (update) - leave it; else use model name
					var locName = ModelImport.conditionString(extras.GetOption("strucLocName"), 256);
					structure.LocalizedName = string.IsNullOrEmpty(locName)
						? (string.IsNullOrEmpty(structure.LocalizedName) ? structure.Name : structure.LocalizedName)
						: locName;

					_clientSession.DbSession.Update(structure);

                    if (dryRun)
                    {
                        tran.Rollback();
                        foreach (string file in _filesSaved)
                        {
                            try { ServiceInstances.InternalFileStorageManager.RemoveFile(file); }
                            catch (FileNotFoundException) { ServiceInstances.FileStorageManager.RemoveFile(file); }
                        }
                        _log.Append("DRY RUN MODE: all changes rolled back.\r\n");
                    }
                    else
                    {
                        //_clientSession.DbSession.Flush();
                        tran.Commit();
                        _log.Append("All changes comitted to database.\r\n");
                    }
                }  // transaction
            }  // client session
        }

		private string storeModelFile(UpdateableBase dbObject, string currentPath,
			string modelFileName, string storePrefix)
		{
			return storeModelFile(ServiceInstances.FileStorageManager, dbObject, currentPath,
				modelFileName, storePrefix, "models");
		}

        private string storeModelFile(IFileStorageManager man, UpdateableBase dbObject, string currentPath,
			string modelFileName, string storePrefix, string namespaceHint)
        {
            string result, proposedName;
			using (var fs = ModelImport.OpenModelFile(modelFileName, out proposedName))
			{
				if (!string.IsNullOrWhiteSpace(currentPath))
					result = man.ReplaceFile(currentPath, namespaceHint, storePrefix, 
						Path.GetExtension(proposedName), dbObject.AutoID.ToString(), fs);
				else
					result = man.StoreFile(namespaceHint, storePrefix, 
						Path.GetExtension(proposedName), dbObject.AutoID.ToString(), fs);
			}
            _filesSaved.Add(result);
            _log.AppendFormat("File persisted ({0}) from {1}\r\n", result, modelFileName);
            return result;
        }
	}
}