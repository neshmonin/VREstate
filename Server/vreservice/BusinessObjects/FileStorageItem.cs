using System.IO;
using System.Security.Cryptography;

namespace Vre.Server.BusinessLogic
{
    internal class FileStorageItem : UpdateableBase
    {
		public enum LocationType : int { Internal = 0, Public = 1 }

        public byte[] Hash { get; private set; }
		private LocationType Store { get; set; }
        public string RelativePath { get; private set; }
        private int UseCounter { get; set; }

		private FileStorageItem() { }

        public FileStorageItem(LocationType store, string location, byte[] hash) : base()
        {
			InitializeNew();
			Store = store;
            RelativePath = location;
			Hash = hash;
            UseCounter = 1;
        }

		/// <summary>
		/// Increment file's use count
		/// </summary>
        public void IncrementUseCount()
        {
            UseCounter++;
            MarkUpdated();
        }

		/// <summary>
		/// Decrement file's use count
		/// </summary>
		/// <returns>true if file is still in use</returns>
        public bool DecrementUseCount()
        {
            UseCounter--;
            if (0 == UseCounter)
            {
                MarkDeleted();
				return false;
            }
            else
            {
                MarkUpdated();
				return true;
            }
        }

		/// <summary>
		/// Set use counter to specific value
		/// </summary>
		/// <param name="count"></param>
		/// <returns>true if value was changed</returns>
		public bool SetUseCount(int count)
		{
			if (count != UseCounter)
			{
				UseCounter = count;
				if (count > 0) Undelete();
				else MarkDeleted();
				return true;
			}
			return false;
		}

		public void ReplaceHash(byte[] newHash)
		{
			Hash = newHash;
			MarkUpdated();
		}
    }
}
