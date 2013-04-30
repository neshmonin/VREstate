using System.Collections.Generic;
namespace Vre.Server.Mls
{
    public interface IMlsInfoProvider
    {
        void Configure(string configurationString);
        string Run();
        IList<string> GetCurrentActiveItems();
        IList<MlsItem> GetNewItems();
    }
}