using System.Collections.Generic;
namespace Vre.Server.Mls
{
    public interface IMlsInfoProvider
    {
        void Configure(string configurationString);
        IList<string> GetCurrentActiveItems();
    }
}