namespace Vre.Server.RemoteService
{
    internal interface IHttpService
    {
        string ServicePathPrefix { get; }
        bool RequiresSession { get; }
        void ProcessCreateRequest(IServiceRequest request);
        void ProcessGetRequest(IServiceRequest request);
        void ProcessReplaceRequest(IServiceRequest request);
        void ProcessDeleteRequest(IServiceRequest request);
    }
}