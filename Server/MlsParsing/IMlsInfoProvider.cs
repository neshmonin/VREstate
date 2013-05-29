using System.Collections.Generic;
using System.IO;

namespace Vre.Server.Mls
{
    public interface IMlsInfoProvider
    {
        void Configure(string configurationString);
        void Run();
		IList<FileInfo> AvailableFiles { get; }
		string Parse();
		string Parse(string fileName);
		IList<string> GetCurrentActiveItems();
        IList<MlsItem> GetNewItems();
    }
}