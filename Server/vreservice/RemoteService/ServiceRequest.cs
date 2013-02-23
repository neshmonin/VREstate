using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using Vre.Server.BusinessLogic;

namespace Vre.Server.RemoteService
{
    /// <summary>
    /// Query parameter storage
    /// </summary>
    internal class ServiceQuery : NameValueCollection
    {
        public ServiceQuery(NameValueCollection fromRequestPri, NameValueCollection fromRequestSec)
            : base(fromRequestSec)
        {
            Add(fromRequestPri);  // duplicated keys get overwritten
        }

        public int GetParam(string name, int defaultValue)
        {
            int result = defaultValue;
            string arg = this[name];
            if (arg != null)
            {
                if (!int.TryParse(arg, out result)) result = defaultValue;
            }
            return result;
        }

        public string GetParam(string name, string defaultValue)
        {
            string result = this[name];
            if (null == result) result = defaultValue;
            return result;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            bool separator = false;
            foreach (string k in Keys)
            {
                if (separator) result.Append("&");
                result.Append(k);
                result.Append("=");
                result.Append(this[k]);
                separator = true;
            }
            return result.ToString();
        }
    }

    internal interface IRemoteUserInfo
    {
        IPEndPoint EndPoint { get; }
        ClientSession Session { get; }
        bool StaleSession { get; }
    }

    internal enum RequestType { Get, Insert, Update, Delete }

    internal interface IRequestData
    {
        RequestType Type { get; }
        string Path { get; }
        ServiceQuery Query { get; }
        ClientData Data { get; }
        byte[] RawData { get; }
        /// <summary>
        /// Construct client's URI for service's root entry point based on request information.
        /// <para>Required to generate a resource for client containing references to server's other resources.</para>
        /// </summary>
        string ConstructClientRootUri();
        bool IsSecureConnection { get; }
    }

    internal interface IResponseData
    {
        HttpStatusCode ResponseCode { get; set; }
        string ResponseCodeDescription { get; set; }
        ClientData Data { get; set; }
        string DataStreamContentType { get; set; }
        string DataPhysicalLocation { get; set; }
        Stream DataStream { get; }
        bool HoldResponseForServerPush { get; set; }
        /// <summary>
        /// Takes precedense over all other response modes.
        /// </summary>
        string RedirectionUrl { get; set; }
    }

    internal interface IServiceRequest
    {
        IRemoteUserInfo UserInfo { get; }
        IRequestData Request { get; }
        IResponseData Response { get; }
    }
}