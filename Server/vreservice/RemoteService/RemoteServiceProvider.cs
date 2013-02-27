using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using Vre.Server.HttpService;

namespace Vre.Server.RemoteService
{
    internal class RemoteServiceProvider : MarshalByRefObject
    {
        private IHttpService _reverseRequestService = new ReverseRequestService();

        public void ProcessRequest(HttpServiceBase server, IServiceRequest request)
        {
            //if (null == _filesRootFolder) initialize();

            switch (request.Request.Type)
            {
                case RequestType.Get:
                    processReadRequest(server, request);
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
            if (request.Request.Path.Equals(ProgramService.ServicePathPrefix))  // special programmatic requests
            {
                ProgramService.ProcessClientRequest(request);
                return;
            }

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

        private void processReadRequest(HttpServiceBase server, IServiceRequest request)
        {
            if (request.Request.Path.StartsWith(_reverseRequestService.ServicePathPrefix))  // programmatic requests, such as login
            {
                _reverseRequestService.ProcessGetRequest(request);
                return;
            }
            if (request.Request.Path.Equals(ProgramService.ServicePathPrefix))  // programmatic requests, such as login
            {
                ProgramService.ProcessClientRequest(request);
                return;
            }
            //else if (request.Request.Path.Equals("test"))  // test ping/pong; LEGACY
            //{
            //    //using (System.IO.StreamWriter w = new System.IO.StreamWriter(request.Response.DataStream))
            //    //    w.WriteLine("VRE Works!");
            //    //request.Response.DataStreamContentType = "txt";
            //    //request.Response.ResponseCode = HttpStatusCode.OK;

            //    // EXPERIMENTAL: starts an HTTP server push response which responds with progressing 
            //    // status pushes until client shuts connection down
            //    request.Response.ResponseCode = HttpStatusCode.OK;
            //    request.Response.HoldResponseForServerPush = true;
            //    request.Response.Data = new BusinessLogic.ClientData();
            //    request.Response.Data.Add("status", 0);

            //    return;
            //}
            //else if (request.Request.Path.Equals("version"))
            //{
            //    byte[] buffer = Encoding.UTF8.GetBytes(BuildVersionInformation());
            //    request.Response.DataStream.Write(buffer, 0, buffer.Length);
            //    request.Response.DataStreamContentType = "txt";
            //    request.Response.ResponseCode = HttpStatusCode.OK;
            //    return;
            //}
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
                server.ProcessFileRequest(request.Request.Path, request.Response);
                return;
            }
        }
    }
}