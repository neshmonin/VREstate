using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace Vre.Server.RemoteService
{
    internal class RemoteServiceProvider : MarshalByRefObject
    {
        private static char[] _invalidPathChars = Path.GetInvalidPathChars();
        private static char[] _invalidFileNameChars = Path.GetInvalidFileNameChars();
        private static List<string> _allowedFileExtensions = new List<string>();
        private static string _filesRootFolder = null;

        private static void initialize()
        {
            _filesRootFolder = ServiceInstances.Configuration.GetValue("FilesRoot",
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            _allowedFileExtensions = Utilities.FromCsv(ServiceInstances.Configuration.GetValue("AllowedServedFileExtensions", string.Empty));
        }

        public static bool IsPathValid(string path, bool allowSubfolder)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;

            if (path.IndexOfAny(_invalidPathChars) >= 0) return false;
            if (!allowSubfolder)
            {
                if (path.IndexOfAny(_invalidFileNameChars) >= 0) return false;
            }

            if (path.Contains("..") || path.StartsWith("\\") || path.StartsWith("/")) return false;

            return true;
        }

        public void ProcessRequest(IServiceRequest request)
        {
            if (null == _filesRootFolder) initialize();

            switch (request.Request.Type)
            {
                case RequestType.Get:
                    processReadRequest(request);
                    break;

                case RequestType.Insert:
                case RequestType.Update:
                case RequestType.Delete:
                    processWriteRequest(request);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private void processWriteRequest(IServiceRequest request)
        {
            // Write request always:
            // - has a sid query parameter (or passed in HTTP header)
            // - refers to a valid active session
            if (null == request.UserInfo.Session) throw new ArgumentException("Need a valid session to perform this operation.");
            // - starts with "data" subpath element referring to some object
            if (!request.Request.Path.StartsWith(DataService.ServicePathPrefix)) throw new ArgumentException("Invalid root path.");

            switch (request.Request.Type)
            {
                case RequestType.Update:
                    DataService.ProcessReplaceRequest(request);
                    break;

                case RequestType.Insert:
                    DataService.ProcessCreateRequest(request);
                    break;

                case RequestType.Delete:
                    DataService.ProcessDeleteRequest(request);
                    break;
            }
        }

        private void processReadRequest(IServiceRequest request)
        {
            if (request.Request.Path.Equals(ProgramService.ServicePathPrefix))  // programmatic requests, such as login
            {
                ProgramService.ProcessClientRequest(request);
                return;
            }
            else if (request.Request.Path.Equals("test"))  // test ping/pong; LEGACY
            {
                //using (System.IO.StreamWriter w = new System.IO.StreamWriter(request.Response.DataStream))
                //    w.WriteLine("VRE Works!");
                //request.Response.DataStreamContentType = "txt";
                //request.Response.ResponseCode = HttpStatusCode.OK;

                // EXPERIMENTAL: starts an HTTP server push response which responds with progressing 
                // status pushes until client shuts connection down
                request.Response.ResponseCode = HttpStatusCode.OK;
                request.Response.HoldResponseForServerPush = true;
                request.Response.Data = new BusinessLogic.ClientData();
                request.Response.Data.Add("status", 0);

                return;
            }
            else if (request.Request.Path.Equals("version"))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(buildVersionInformation());
                request.Response.DataStream.Write(buffer, 0, buffer.Length);
                request.Response.DataStreamContentType = "txt";
                request.Response.ResponseCode = HttpStatusCode.OK;
                return;
            }
            else if (request.Request.Path.StartsWith(GenerationService.ServicePathPrefix))  // on-the-fly generated content (requires session)
            {
                GenerationService.ProcessClientRequest(request);
                return;
            }
            else if (request.Request.Path.StartsWith(DataService.ServicePathPrefix))  // object reading (requires session)
            {
                // Write request always:
                // - has a sid query parameter (or passed in HTTP header)
                // - refers to a valid active session
                if (null == request.UserInfo.Session) throw new ArgumentException("Need a valid session to perform this operation.");

                DataService.ProcessGetRequest(request);
                return;
            }
            else //if (0 == request.Request.Query.Count)  // FS file reading
            {
                string type, location;
                processFileRequest(request.Request.Path, out type, out location);
                request.Response.DataStreamContentType = type;
                request.Response.DataPhysicalLocation = location;
                request.Response.ResponseCode = HttpStatusCode.OK;
                return;
            }
        }

        private void processFileRequest(string file, out string resourceType, out string resourcePath)
        {
            // validate path
            if (!IsPathValid(file, true)) throw new FileNotFoundException("Path is invalid.");

            // validate file type: only certain file types are accessible (!)
            resourceType = Path.GetExtension(file).ToLower().Substring(1);
            if (!_allowedFileExtensions.Contains(resourceType)) throw new FileNotFoundException("File type not known.");

            // verify file presence
            resourcePath = Path.Combine(_filesRootFolder, file);
            if (!File.Exists(resourcePath)) throw new FileNotFoundException("File not found.");
        }

        private static string buildVersionInformation()
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("{0}, {1}", VersionGen.ProductName, VersionGen.CopyrightString);

            result.AppendFormat("\r\nVersion: {0}", Assembly.GetExecutingAssembly().GetName().Version);
            result.AppendFormat("\r\nBuild version stamp: {0}", VersionGen.VersionStamp);

            return result.ToString();
        }
    }
}