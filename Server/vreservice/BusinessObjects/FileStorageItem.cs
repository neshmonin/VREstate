using System.IO;
using System.Security.Cryptography;

namespace Vre.Server.BusinessLogic
{
    internal class FileStorageItem : UpdateableBase
    {
        private byte[] Hash { get; set; }
        private string Location { get; set; }
        private int UseCounter { get; set; }

        public FileStorageItem(string location) : base()
        {
            Location = location;
            UseCounter = 1;

            using (Stream file = ServiceInstances.FileStorageManager.OpenFile(Location))
            {
                Hash = CalcHash(file);
            }
        }

        public void IncrementUseCount()
        {
            UseCounter++;
            MarkUpdated();
        }

        public void DecrementUseCount()
        {
            UseCounter--;
            if (0 == UseCounter)
            {
                MarkDeleted();
                ServiceInstances.FileStorageManager.RemoveFile(Location);
            }
            else
            {
                MarkUpdated();
            }
        }

        public bool CompareWith(Stream candidate)
        {
            bool result = true;
            
            using (Stream original = ServiceInstances.FileStorageManager.OpenFile(Location))
            {
                byte[] obuffer = new byte[8192];
                byte[] cbuffer = new byte[obuffer.Length];
                int rd;

                do
                {
                    rd = original.Read(obuffer, 0, obuffer.Length);

                    if (candidate.Read(cbuffer, 0, cbuffer.Length) != rd)
                    { result = false; break; }

                    for (int idx = rd - 1; idx >= 0; idx--)
                        if (obuffer[idx] != cbuffer[idx]) { result = false; break; }
                }
                while (rd > 0);
            }

            return result;
        }

        public static byte[] CalcHash(Stream file)
        {
            using (MD5 hash = MD5.Create())
            {
                hash.Initialize();
                return hash.ComputeHash(file);
            }
        }
    }
}
