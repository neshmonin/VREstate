using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Xml;
using Vre.Server.RemoteService;

namespace Vre.Server.HttpService
{
    internal class RedirectionService : HttpServiceBase
    {
        enum ServiceType { Unknown, /*Const,*/ Redirection, Button, Reverse, ServerSessionStatus, Reboot }
        private const string _defaultImage = "default.png";

        private string _buttonStorePath;
        private AliasMap _map, _testMap;
        private ButtonStoreFsNameCache _imagePathCache;

        public RedirectionService() : base("HTTP Redirection")
        {
			// TODO: Static configuration

			_buttonStorePath = Configuration.Redirector.ButtonStorePath.Value;
            if (string.IsNullOrWhiteSpace(_buttonStorePath))
            {
                _buttonStorePath = null;
                _imagePathCache = null;
                ServiceInstances.Logger.Error("RedirectorButtonsStore parameter is not set; buttons shall not be provided.");
            }
            else
            {
                _imagePathCache = new ButtonStoreFsNameCache(_buttonStorePath);
            }

            _map = new AliasMap(Configuration.Redirector.Aliases.ProductionMapFile.Value);
            _testMap = new AliasMap(Configuration.Redirector.Aliases.TestingMapFile.Value);
        }

        protected override IResponseData process(string browserKey, HttpListenerContext ctx, HttpServiceRequest.ProcessResponse proc)
        {
            IResponseData result = new HttpServiceRequest.ResponseData(new MemoryStream(0), ctx.Response, proc);

            if (ctx.Request.HttpMethod.Equals("GET", StringComparison.InvariantCultureIgnoreCase))
            {
                if (Configuration.Debug.AllowExtendedLogging.Value)
                {
                    if (Configuration.Debug.AllowReallyExtendedLogging.Value)
                        ServiceInstances.RequestLogger.Info("{0}: {1} {2} {3} {4} {5} {6}", browserKey,
                            ctx.Request.Url, ctx.Request.RemoteEndPoint, ctx.Request.UserAgent,
                            ctx.Request.UserHostAddress, ctx.Request.UserHostName, ctx.Request.UrlReferrer);
                    else
                        ServiceInstances.RequestLogger.Info("{0}: {1}", browserKey, ctx.Request.Url);
                }

                string path = ctx.Request.Url.LocalPath;
                if (path.StartsWith(_path))
                {
                    path = path.Remove(0, _path.Length);
                    if (path.Length > 0)
                    {
                        switch (serviceTypeByName(path))
                        {
                            case ServiceType.Redirection:
                                result.RedirectionUrl = redirect(ctx, browserKey);
                                break;

                            case ServiceType.Reverse:
                                result.RedirectionUrl = reverse(ctx, browserKey);
                                break;

                            case ServiceType.Button:
                                processButtonRequest(ctx, browserKey, result);
                                break;

                            case ServiceType.ServerSessionStatus:
                                processServerSessionStatus(ctx, path, result);
                                break;

							case ServiceType.Reboot:
								processSystemReboot(ctx, path, result);
								break;
                            //case ServiceType.Const:
                            //    processConst(ctx, path, result);
                            //    break;

                            default:
								var redirectUrl = altRedirect(ctx, path, browserKey);
								if (null != redirectUrl) result.RedirectionUrl = redirectUrl;
								else ProcessFileRequest(path, result);
                                break;
                        }
                    }
                    else  // path is empty; test or error; redirect to main site
                    {
                        result.RedirectionUrl = Configuration.Redirector.Urls.DefaultUrl.Value;
                    }
                }
                else
                {
					result.RedirectionUrl = Configuration.Redirector.Urls.DefaultUrl.Value;
                }
            }
            else
            {
                result.ResponseCode = HttpStatusCode.NotImplemented;
            }

            return result;
        }

        private static ServiceType serviceTypeByName(string pathElement)
        {
            if (pathElement.Equals("start")) return ServiceType.Redirection;
            else if (pathElement.Equals("button")) return ServiceType.Button;
            else if (pathElement.Equals("go")) return ServiceType.Reverse;
            //else if (pathElement.Equals("robots.txt")) return ServiceType.Const;
            //else if (pathElement.Equals("humans.txt")) return ServiceType.Const;
            //else if (pathElement.Equals("version")) return ServiceType.Const;
            else if (pathElement.Equals("sss")) return ServiceType.ServerSessionStatus;
			else if (pathElement.Equals("reboot")) return ServiceType.Reboot;
			return ServiceType.Unknown;
        }

        private string redirect(HttpListenerContext ctx, string browserKey)
        {
            string finalUri = null;
            bool testMode = false;

            foreach (string k in ctx.Request.QueryString.AllKeys)
            {
                if (k.Equals("project"))
                {
                    finalUri = ctx.Request.QueryString[k];
                }
                else if (k.Equals("test"))
                {
                    testMode = ctx.Request.QueryString[k].Equals("true");
                }
            }

            if (finalUri != null) finalUri = testMode ? _testMap.UriByAlias(finalUri) : _map.UriByAlias(finalUri);
			if (null == finalUri) finalUri = Configuration.Redirector.Urls.DefaultUrl.Value;

            string queryString = ctx.Request.Url.Query;
            if (queryString.Length > 0)
            {
                if (finalUri.Contains("?")) finalUri += "&" + queryString.Substring(1);
                else finalUri += queryString;
            }

            //ctx.Request.UrlReferrer;
            // TODO: save statistics
            //ctx.Response.Redirect(finalUri);
            return finalUri;
        }

		private string altRedirect(HttpListenerContext ctx, string pathElementExtracted, string browserKey)
		{
			string finalUri;
			bool testMode = false;

			foreach (string k in ctx.Request.QueryString.AllKeys)
			{
				if (k.Equals("test"))
				{
					testMode = ctx.Request.QueryString[k].Equals("true");
				}
			}

			finalUri = testMode ? _testMap.UriByAlias(pathElementExtracted) : _map.UriByAlias(pathElementExtracted);

			if (finalUri != null)
			{
				string queryString = ctx.Request.Url.Query;
				if (queryString.Length > 0)
				{
					if (finalUri.Contains("?")) finalUri += "&" + queryString.Substring(1);
					else finalUri += queryString;
				}
			}

			//ctx.Request.UrlReferrer;
			// TODO: save statistics
			//ctx.Response.Redirect(finalUri);
			return finalUri;
		}

		private string reverse(HttpListenerContext ctx, string browserKey)
        {
            bool testMode = false;
            string id = null;

			var queryString = new StringBuilder();
			var queryStringEmpty = true;

            foreach (var k in ctx.Request.QueryString.AllKeys)
            {
				var v = ctx.Request.QueryString[k];
				var passThrough = true;

                if (k.Equals("id"))
                {
                    id = v;
					passThrough = false;
                }
                else if (k.Equals("test"))
                {
                    testMode = v.Equals("true");
                }

				if (passThrough)
				{
					queryString.AppendFormat("{0}{1}={2}", 
						queryStringEmpty ? string.Empty : "&", k, v);

					queryStringEmpty = false;
				}
            }

            string finalUri;

            if (null == id)
            {
				finalUri = Configuration.Redirector.Urls.DefaultUrl.Value;
            }
            else
            {
                switch (UniversalId.TypeInUrlId(id))
                {
                    default:  // legacy
                    case UniversalId.IdType.ViewOrder:
						finalUri = string.Format(testMode 
							? Configuration.Redirector.Urls.TestingClientViewOrderTemplate.Value
							: Configuration.Redirector.Urls.ProductionClientViewOrderTemplate.Value, id);
                        break;

                    case UniversalId.IdType.ReverseRequest:
                        finalUri = string.Format(testMode
							? Configuration.Redirector.Urls.TestingReverseRequestTemplate.Value
							: Configuration.Redirector.Urls.ProductionReverseRequestTemplate.Value, id);
                        break;
                }
            }

			if (queryString.Length > 0)
			{
				if (finalUri.Contains("?")) finalUri += "&";
				finalUri += queryString;
			}
			
			//ctx.Request.UrlReferrer;
            // TODO: save statistics
            //ctx.Response.Redirect(finalUri);
            return finalUri;
        }

        private void processButtonRequest(HttpListenerContext ctx, string browserKey, IResponseData response)
        {
            string aliasName = null;
            string imageName = "default";

            foreach (string k in ctx.Request.QueryString.AllKeys)
            {
                if (k.Equals("project"))
                {
                    aliasName = ctx.Request.QueryString[k];
                    if (null == _map.UriByAlias(aliasName)) aliasName = null;
                }
                else if (k.Equals("image"))
                {
                    imageName = ctx.Request.QueryString[k];
                }
            }
            
            bool served = false;

            if (aliasName != null)
            {
                if (!IsPathValid(imageName, false)) imageName = "default";
                
                string fullPath = (_imagePathCache != null) ? _imagePathCache.PathByHint(aliasName, imageName) : null;

                if (fullPath != null)
                {
                    response.DataPhysicalLocation = fullPath;
                    streamImage(fullPath, response);
                    served = true;
                }
            }

            if (!served)
                streamImage(Path.Combine(_buttonStorePath, _defaultImage), response);

            //ctx.Request.UrlReferrer;
            // TODO: save statistics

        }

        //private static void processConst(HttpListenerContext ctx, string path, IResponseData response)
        //{
        //    response.ResponseCode = HttpStatusCode.OK;
        //    response.DataStreamContentType = "txt";

        //    StringBuilder text = new StringBuilder();

        //    if (path.Equals("robots.txt"))
        //    {
        //        text.Append("User-agent: *\r\n");
        //        text.Append("Disallow: /\r\n");
        //        text.Append("Disallow: /harming/humans\r\n");
        //        text.Append("Disallow: /ignoring/human/orders\r\n");
        //        text.Append("Disallow: /harm/to/self\r\n");
        //    }
        //    else if (path.Equals("humans.txt"))
        //    {
        //        if (DateTime.MinValue == _buildTime)
        //        {
        //            _buildTime = File.GetCreationTime(Assembly.GetExecutingAssembly().Location);
        //        }

        //        text.Append("/* humanstxt.org */\r\n");
        //        text.Append("\r\n");
        //        text.Append("/* TEAM */\r\n");
        //        text.Append("\tAlexander Neshmonin, CEO and everything, Toronto\r\n");
        //        text.Append("\tEugene Simonov, Frontend, Ukraine\r\n");
        //        text.Append("\tAndrey Maslyuk, Backend, Toronto\r\n");
        //        text.Append("\r\n");
        //        text.Append("/* THANKS */\r\n");
        //        text.Append("\tVitaly Zholudev\r\n");
        //        text.Append("\thttp://last.fm\r\n");
        //        text.Append("\thttp://stackoverflow.com\r\n");
        //        text.Append("\r\n");
        //        text.Append("\r\n");
        //        text.Append("/* SITE */\r\n");
        //        text.Append("\tLast build: " + _buildTime.ToShortDateString() + "\r\n");
        //        text.Append("\tLast update: today\r\n");
        //        text.Append("\tStandards: HTTP/1.1\r\n");
        //        text.Append("\tLanguages: none\r\n");
        //    }
        //    else if (path.Equals("version"))
        //    {
        //        text.Append(RemoteServiceProvider.BuildVersionInformation());
        //    }

        //    byte[] buffer = Encoding.UTF8.GetBytes(text.ToString());
        //    response.DataStream.Write(buffer, 0, buffer.Length);
        //}

        //private string deriveExtension(string aliasName, string imageName)
        //{
        //    string result = null;

        //    try
        //    {
        //        foreach (string file in Directory.EnumerateFiles(
        //            Path.Combine(_buttonStorePath, aliasName), imageName + ".*", SearchOption.TopDirectoryOnly))
        //        {
        //            result = Path.GetExtension(file);
        //            break;
        //        }
        //    }
        //    catch (DirectoryNotFoundException) { }

        //    return result;
        //}

        private static void processServerSessionStatus(HttpListenerContext ctx, string path, IResponseData response)
        {
            response.ResponseCode = HttpStatusCode.OK;
            response.DataStreamContentType = "txt";

            StringBuilder text = new StringBuilder();

            var sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            var mutexsecurity = new MutexSecurity();
            mutexsecurity.AddAccessRule(new MutexAccessRule(sid, MutexRights.FullControl, AccessControlType.Allow));
            mutexsecurity.AddAccessRule(new MutexAccessRule(sid, MutexRights.ChangePermissions, AccessControlType.Deny));
            mutexsecurity.AddAccessRule(new MutexAccessRule(sid, MutexRights.Delete, AccessControlType.Deny));

            bool created, result;
            using (System.Threading.Mutex m = new System.Threading.Mutex(false, "Global\\788c23ce-8a6d-4024-b498-2f1bace634f2", out created, mutexsecurity))
            {
                result = m.WaitOne(0);
                if (result) m.ReleaseMutex();
            }

            if (result)
                text.Append("Server is vacant.\r\n");
            else
                text.Append("Server is BUSY.\r\n");

            byte[] buffer = Encoding.UTF8.GetBytes(text.ToString());
            response.DataStream.Write(buffer, 0, buffer.Length);
        }

		private static void processSystemReboot(HttpListenerContext ctx, string path, IResponseData response)
		{
			response.ResponseCode = HttpStatusCode.OK;
			response.DataStreamContentType = "txt";

			StringBuilder text = new StringBuilder();

			try
			{
				ServiceInstances.Logger.Info("Remote reboot request from {0}", ctx.Request.RemoteEndPoint);

				bool force0 = !string.IsNullOrWhiteSpace(ctx.Request.QueryString["force0"]);
				bool force1 = !string.IsNullOrWhiteSpace(ctx.Request.QueryString["force1"]);

				long key;
				if (long.TryParse(ctx.Request.QueryString["key"], out key))
				{
					var now = DateTime.Now;

					long realKey = now.Year % 100 + now.Month + now.Day + now.Hour + now.Minute;

					if (realKey == key)
					{
						var command = RebootManager.ExitWindows.Reboot;

						if (force0) command |= RebootManager.ExitWindows.Force;
						else if (force1) command |= RebootManager.ExitWindows.ForceIfHung;

						if (RebootManager.PowerUtilities.ExitWindows(command, RebootManager.ShutdownReason.FlagPlanned, true))
							text.Append("Rebooting now.\r\n");
						else
							text.Append("Rebooting failed.\r\n");
					}
				}

			}
			catch (Exception ex)
			{
				ServiceInstances.Logger.Error("Reboot processing failed: {0}\r\n{1}", ex.Message, ex.StackTrace);
			}

			byte[] buffer = Encoding.UTF8.GetBytes(text.ToString());
			response.DataStream.Write(buffer, 0, buffer.Length);
		}

		private static void streamImage(string path, IResponseData response)
        {
            response.ResponseCode = HttpStatusCode.OK;
            response.DataStreamContentType = Path.GetExtension(path).ToLower().Substring(1);
            response.DataPhysicalLocation = path;
        }
    }

    internal class ButtonStoreFsNameCache
    {
        private static Dictionary<string, string> _cache = new Dictionary<string, string>();
        private FileSystemWatcher _watcher;
        private string _path;

        public ButtonStoreFsNameCache(string path)
        {
            _path = path;
            if (_path.EndsWith(new string(Path.DirectorySeparatorChar, 1))) _path = _path.Substring(0, _path.Length - 1);

            _watcher = null;
            initializeWatcher();
        }

        public string PathByHint(string aliasName, string imageName)
        {
            string result;
            string hint = Path.Combine(aliasName, imageName);
            lock (_cache)
            {
                if (!_cache.TryGetValue(hint, out result))
                {
                    result = deriveExtension(aliasName, imageName);
                    _cache.Add(hint, result);
                }
            }
            return result;
        }

        private string deriveExtension(string aliasName, string imageName)
        {
            string result = null;

            try
            {
                foreach (string file in Directory.EnumerateFiles(
                    Path.Combine(_path, aliasName), imageName + ".*", SearchOption.TopDirectoryOnly))
                {
                    result = file;
                    break;
                }
            }
            catch (DirectoryNotFoundException) { }

            return result;
        }

        private void initializeWatcher()
        {
            lock (this)
            {
                if (_watcher != null) return;

                _watcher = new FileSystemWatcher(_path);
                _watcher.Filter = "*.*";
                _watcher.IncludeSubdirectories = true;
                _watcher.EnableRaisingEvents = false;

                //_watcher.Changed += new FileSystemEventHandler(_watcher_CCD);
                _watcher.Created += new FileSystemEventHandler(_watcher_Created);
                _watcher.Deleted += new FileSystemEventHandler(_watcher_Deleted);
                _watcher.Renamed += new RenamedEventHandler(_watcher_Renamed);
                _watcher.Error += new ErrorEventHandler(_watcher_Error);

                _watcher.EnableRaisingEvents = true;
            }
        }

        private void _watcher_Error(object sender, ErrorEventArgs e)
        {
            lock (sender)
            {
                ServiceInstances.Logger.Error("BFS: File system watcher failed.  Recreating.\r\n{0}", e.GetException());
                try { _watcher.Dispose(); }
                catch { }
                _watcher = null;
                initializeWatcher();
            }
        }

        private void _watcher_Created(object sender, FileSystemEventArgs e)
        {
            lock (sender)
            {
                ServiceInstances.Logger.Info("BSC: Detected added file: \"{0}\"", e.FullPath);
                processAdd(e.FullPath);
            }
        }

        private void _watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            lock (sender)
            {
                ServiceInstances.Logger.Info("BSC: Detected file removal: \"{0}\"", e.FullPath);
                processDelete(e.FullPath);
            }
        }

        private void _watcher_Renamed(object sender, RenamedEventArgs e)
        {
            lock (sender)
            {
                ServiceInstances.Logger.Info("BSC: Detected file rename: \"{0}\" -> \"{1}\"", e.OldFullPath, e.FullPath);
                processDelete(e.OldFullPath);
                processAdd(e.FullPath);
            }
        }

        private void processAdd(string fullPath)
        {
            string fullPathNoExtension = Path.Combine(Path.GetDirectoryName(fullPath),
                Path.GetFileNameWithoutExtension(fullPath));
            if (fullPathNoExtension.StartsWith(_path, StringComparison.InvariantCultureIgnoreCase))
            {
                string createdHint = fullPathNoExtension.Substring(_path.Length + 1);
                lock (_cache)
                {
                    string foundKey = null;
                    foreach (string hint in _cache.Keys)
                    {
                        if (hint.Equals(createdHint, StringComparison.InvariantCultureIgnoreCase))
                        {
                            foundKey = hint;
                            break;
                        }
                    }
                    if (foundKey != null)
                    {
                        if (null == _cache[foundKey]) _cache[foundKey] = fullPath;
                        // otherwise it is a duplicate with a different extension
                    }
                }
            }
        }

        private void processDelete(string fullPath)
        {
            lock (_cache)
            {
                string foundKey = null;
                foreach (KeyValuePair<string, string> kvp in _cache)
                {
                    if (null == kvp.Value) continue;
                    if (kvp.Value.Equals(fullPath, StringComparison.InvariantCultureIgnoreCase))
                    {
                        foundKey = kvp.Key;
                        break;
                    }
                }
                if (foundKey != null)
                {
                    string[] path = foundKey.Split(Path.DirectorySeparatorChar);
                    if (2 == path.Length)
                    {
                        // try looking for other files with same name and different extension
                        _cache[foundKey] = deriveExtension(path[0], path[1]);
                    }
                    else  // dunno what to do!
                    {
                        _cache[foundKey] = null;
                        ServiceInstances.Logger.Error("BSC: Found an unknowk cache key type: '{0}'", foundKey);
                    }
                }
            }
        }
    }

	/// <summary>
	/// http://stackoverflow.com/questions/24726116/when-using-exitwindowsex-logoff-works-but-shutdown-and-restart-do-not
	/// </summary>
	internal class RebootManager
	{
		public static class PowerUtilities
		{
			[DllImport("user32.dll", SetLastError = true)]
			private static extern int ExitWindowsEx(ExitWindows uFlags, ShutdownReason dwReason);

			public static bool ExitWindows(ExitWindows exitWindows, ShutdownReason reason, bool ajustToken)
			{
				if (ajustToken && !TokenAdjuster.EnablePrivilege("SeShutdownPrivilege", true))
				{
					return false;
				}

				return ExitWindowsEx(exitWindows, reason) != 0;
			}
		}

		[Flags]
		public enum ExitWindows : uint
		{
			// ONE of the following:
			LogOff = 0x00,
			ShutDown = 0x01,
			Reboot = 0x02,
			PowerOff = 0x08,
			RestartApps = 0x40,
			// plus AT MOST ONE of the following two:
			Force = 0x04,
			ForceIfHung = 0x10,
		}

		[Flags]
		public enum ShutdownReason : uint
		{
			None = 0,

			MajorApplication = 0x00040000,
			MajorHardware = 0x00010000,
			MajorLegacyApi = 0x00070000,
			MajorOperatingSystem = 0x00020000,
			MajorOther = 0x00000000,
			MajorPower = 0x00060000,
			MajorSoftware = 0x00030000,
			MajorSystem = 0x00050000,

			MinorBlueScreen = 0x0000000F,
			MinorCordUnplugged = 0x0000000b,
			MinorDisk = 0x00000007,
			MinorEnvironment = 0x0000000c,
			MinorHardwareDriver = 0x0000000d,
			MinorHotfix = 0x00000011,
			MinorHung = 0x00000005,
			MinorInstallation = 0x00000002,
			MinorMaintenance = 0x00000001,
			MinorMMC = 0x00000019,
			MinorNetworkConnectivity = 0x00000014,
			MinorNetworkCard = 0x00000009,
			MinorOther = 0x00000000,
			MinorOtherDriver = 0x0000000e,
			MinorPowerSupply = 0x0000000a,
			MinorProcessor = 0x00000008,
			MinorReconfig = 0x00000004,
			MinorSecurity = 0x00000013,
			MinorSecurityFix = 0x00000012,
			MinorSecurityFixUninstall = 0x00000018,
			MinorServicePack = 0x00000010,
			MinorServicePackUninstall = 0x00000016,
			MinorTermSrv = 0x00000020,
			MinorUnstable = 0x00000006,
			MinorUpgrade = 0x00000003,
			MinorWMI = 0x00000015,

			FlagUserDefined = 0x40000000,
			FlagPlanned = 0x80000000
		}

		public sealed class TokenAdjuster
		{
			// PInvoke stuff required to set/enable security privileges
			private const int SE_PRIVILEGE_ENABLED = 0x00000002;
			private const int TOKEN_ADJUST_PRIVILEGES = 0X00000020;
			private const int TOKEN_QUERY = 0X00000008;
			private const int TOKEN_ALL_ACCESS = 0X001f01ff;
			private const int PROCESS_QUERY_INFORMATION = 0X00000400;

			[DllImport("advapi32", SetLastError = true), SuppressUnmanagedCodeSecurity]
			private static extern int OpenProcessToken(
				IntPtr ProcessHandle, // handle to process
				int DesiredAccess, // desired access to process
				ref IntPtr TokenHandle // handle to open access token
				);

			[DllImport("kernel32", SetLastError = true), SuppressUnmanagedCodeSecurity]
			private static extern bool CloseHandle(IntPtr handle);

			[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			private static extern int AdjustTokenPrivileges(
				IntPtr TokenHandle,
				int DisableAllPrivileges,
				IntPtr NewState,
				int BufferLength,
				IntPtr PreviousState,
				ref int ReturnLength);

			[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			private static extern bool LookupPrivilegeValue(
				string lpSystemName,
				string lpName,
				ref LUID lpLuid);

			public static bool EnablePrivilege(string lpszPrivilege, bool bEnablePrivilege)
			{
				bool retval = false;
				int ltkpOld = 0;
				IntPtr hToken = IntPtr.Zero;
				TOKEN_PRIVILEGES tkp = new TOKEN_PRIVILEGES();
				tkp.Privileges = new int[3];
				TOKEN_PRIVILEGES tkpOld = new TOKEN_PRIVILEGES();
				tkpOld.Privileges = new int[3];
				LUID tLUID = new LUID();
				tkp.PrivilegeCount = 1;
				if (bEnablePrivilege)
					tkp.Privileges[2] = SE_PRIVILEGE_ENABLED;
				else
					tkp.Privileges[2] = 0;
				if (LookupPrivilegeValue(null, lpszPrivilege, ref tLUID))
				{
					Process proc = Process.GetCurrentProcess();
					if (proc.Handle != IntPtr.Zero)
					{
						if (OpenProcessToken(proc.Handle, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY,
							ref hToken) != 0)
						{
							tkp.PrivilegeCount = 1;
							tkp.Privileges[2] = SE_PRIVILEGE_ENABLED;
							tkp.Privileges[1] = tLUID.HighPart;
							tkp.Privileges[0] = tLUID.LowPart;
							const int bufLength = 256;
							IntPtr tu = Marshal.AllocHGlobal(bufLength);
							Marshal.StructureToPtr(tkp, tu, true);
							if (AdjustTokenPrivileges(hToken, 0, tu, bufLength, IntPtr.Zero, ref ltkpOld) != 0)
							{
								// successful AdjustTokenPrivileges doesn't mean privilege could be changed
								if (Marshal.GetLastWin32Error() == 0)
								{
									retval = true; // Token changed
								}
							}
							TOKEN_PRIVILEGES tokp = (TOKEN_PRIVILEGES)Marshal.PtrToStructure(tu, typeof(TOKEN_PRIVILEGES));
							Marshal.FreeHGlobal(tu);
						}
					}
				}
				if (hToken != IntPtr.Zero)
				{
					CloseHandle(hToken);
				}
				return retval;
			}

			[StructLayout(LayoutKind.Sequential)]
			internal struct LUID
			{
				internal int LowPart;
				internal int HighPart;
			}

			[StructLayout(LayoutKind.Sequential)]
			private struct LUID_AND_ATTRIBUTES
			{
				private LUID Luid;
				private int Attributes;
			}

			[StructLayout(LayoutKind.Sequential)]
			internal struct TOKEN_PRIVILEGES
			{
				internal int PrivilegeCount;
				[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
				internal int[] Privileges;
			}

			[StructLayout(LayoutKind.Sequential)]
			private struct _PRIVILEGE_SET
			{
				private int PrivilegeCount;
				private int Control;

				[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)] // ANYSIZE_ARRAY = 1
				private LUID_AND_ATTRIBUTES[] Privileges;
			}
		}
	}

    internal class AliasMap
    {
        private FileSystemWatcher _watcher;
        private string _filePath;
        private Dictionary<string, string> _map;

        public AliasMap(string fileName)
        {
            _filePath = fileName;

            if (!Path.IsPathRooted(_filePath)) 
                _filePath = Path.Combine(Configuration.ConfigurationFilesPath, _filePath);

            _map = null;
            _watcher = null;
            initializeWatcher();
        }

        public string UriByAlias(string alias)
        {
            lock (this) 
            {
                string result;
                if (!_map.TryGetValue(alias, out result)) result = null;
                return result;
            }
        }

        private void initializeWatcher()
        {
            lock (this)
            {
                if (_watcher != null) return;

                _watcher = new FileSystemWatcher(Path.GetDirectoryName(_filePath));
                _watcher.Filter = Path.GetFileName(_filePath);
                _watcher.IncludeSubdirectories = false;
                _watcher.EnableRaisingEvents = false;

                _watcher.Changed += new FileSystemEventHandler(_watcher_Changed);
                _watcher.Created += new FileSystemEventHandler(_watcher_Created);
                _watcher.Deleted += new FileSystemEventHandler(_watcher_Deleted);
                _watcher.Error += new ErrorEventHandler(_watcher_Error);

                ServiceInstances.Logger.Info("AC: Started reading alias map.");
                reReadMap();
                ServiceInstances.Logger.Info("AC: Reading alias map done.");

                _watcher.EnableRaisingEvents = true;
            }
        }

        private void _watcher_Error(object sender, ErrorEventArgs e)
        {
            lock (this)
            {
                ServiceInstances.Logger.Error("AC: File system watcher failed.  Recreating.\r\n{0}", e.GetException());
                try { _watcher.Dispose(); }
                catch { }
                _watcher = null;
                initializeWatcher();
            }
        }

        private void _watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            lock (this)
            {
                ServiceInstances.Logger.Info("AC: Detected removal of alias map: \"{0}\"", e.FullPath);
                // NOOP; cache?
            }
        }

        private void _watcher_Created(object sender, FileSystemEventArgs e)
        {
            lock (this)
            {
                ServiceInstances.Logger.Info("AC: Started reading alias map.");
                reReadMap();
                ServiceInstances.Logger.Info("AC: Reading alias map done.");
            }
        }

        private void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            lock (this)
            {
                ServiceInstances.Logger.Info("AC: Started reading alias map.");
                reReadMap();
                ServiceInstances.Logger.Info("AC: Reading alias map done.");
            }
        }

        private void reReadMap()
        {
            Dictionary<string, string> newMap = new Dictionary<string, string>();
            bool result = false;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(_filePath);

                XmlNode rootNode = doc.FirstChild;
                while (rootNode != null)
                {
                    if (rootNode.Name.Equals("configuration")) break;
                    rootNode = rootNode.NextSibling;
                }

                if (rootNode != null)
                {
                    XmlNode node = rootNode.FirstChild;

                    while (node != null)
                    {
                        if (node.Name.Equals("alias"))
                        {
                            XmlAttribute attr;
                            string name = null, uri = null;

                            attr = node.Attributes["name"];
                            if (attr != null) name = attr.Value;

                            attr = node.Attributes["uri"];
                            if (attr != null) uri = attr.Value;

                            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(uri))
                                newMap.Add(name, uri);
                        }

                        node = node.NextSibling;
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                ServiceInstances.Logger.Error("Cannot read alias map file ({0}): {1}", _filePath, ex);
            }

            lock (this) 
            {
                if (result) _map = newMap;
                else if (null == _map) _map = new Dictionary<string, string>();
            }
        }
    }
}