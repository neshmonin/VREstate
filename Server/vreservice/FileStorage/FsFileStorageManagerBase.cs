using System;
using System.IO;
using System.Text;

namespace Vre.Server.FileStorage
{
    /// <summary>
    /// Base class for file storage manager using local file system
    /// </summary>
    internal abstract class FsFileStorageManagerBase : IFileStorageManager
    {
        /// <summary>
        /// List of invalid file name characters
        /// </summary>
        protected static readonly char[] _invalidNameChars = Path.GetInvalidFileNameChars();
        /// <summary>
        /// List of invalid path name characters
        /// </summary>
        protected static readonly char[] _invalidPathChars = Path.GetInvalidPathChars();
        /// <summary>
        /// Root path of local file system storage
        /// </summary>
        protected string _storeRoot = null;

        /// <summary>
        /// Iinitializes base class members
        /// </summary>
        /// <param name="fileSystemRootConfigParamName">Configuration file parameter holding root path of local file system storage</param>
        public FsFileStorageManagerBase(string fileSystemRootConfigParamName)
        {
            _storeRoot = ServiceInstances.Configuration.GetValue(fileSystemRootConfigParamName, "");

            if (string.IsNullOrWhiteSpace(_storeRoot))
            {
                ServiceInstances.Logger.Error("File Store settings ({0}) not defined; local file store is disabled.",
                    fileSystemRootConfigParamName);
                _storeRoot = null;
            }
            else
            {
                if (!Directory.Exists(_storeRoot))
                {
                    try { Directory.CreateDirectory(_storeRoot); }
                    catch (Exception ex)
                    {
                        ServiceInstances.Logger.Error(
                            "File Store root folder ({0}) cannot be created; local file store is disabled.\r\n{1}",
                            _storeRoot, Utilities.ExplodeException(ex));
                        _storeRoot = null;
                    }
                }
            }
        }

        /// <summary>
        /// Store file in local file system; returns stored file path relative to store root.
        /// </summary>
        protected string storeFile(string namespaceHint, string typeHint, string extension, string idHint, Stream data)
        {
            if (null == _storeRoot) throw new NotSupportedException("File Store is disabled");

            // pre-validation
            //
            if (!string.IsNullOrWhiteSpace(extension))
                if (extension.IndexOfAny(_invalidNameChars) >= 0) 
                    throw new ApplicationException("Extension provided is not valid");

            // sanitizing
            //
            if (!string.IsNullOrWhiteSpace(namespaceHint)) sanitizePathElement(ref namespaceHint, _invalidPathChars);
            if (!string.IsNullOrWhiteSpace(typeHint)) sanitizePathElement(ref typeHint, _invalidNameChars);
            if (!string.IsNullOrWhiteSpace(idHint)) sanitizePathElement(ref idHint, _invalidNameChars);

            // building relative path
            //
            StringBuilder relativePath = new StringBuilder();

            // add namespace as subfolder if provided
            if (!string.IsNullOrWhiteSpace(namespaceHint))
            {
                relativePath.Append(namespaceHint);
                relativePath.Append(Path.DirectorySeparatorChar);
            }

            if (!string.IsNullOrWhiteSpace(idHint))
                relativePath.Append(idHint);
            else
                relativePath.Append(Guid.NewGuid().ToString("N"));

            if (!string.IsNullOrWhiteSpace(typeHint))
            {
                relativePath.Append('_');
                relativePath.Append(typeHint);
            }

            if (!string.IsNullOrWhiteSpace(extension))
            {
                if (!extension.StartsWith(".")) relativePath.Append('.');
                relativePath.Append(extension);
            }
            else
            {
                relativePath.Append(".bin");
            }

            // avoiding collisions
            //
            string rp = relativePath.ToString();
            if (File.Exists(Path.Combine(_storeRoot, rp)))
            {
                int idx = rp.LastIndexOf('.');
                // append with date/time first
                rp = rp.Substring(0, idx)
                    + "_"
                    + DateTime.UtcNow.ToString("yyyyMMddHHmmss")
                    + rp.Substring(idx);

                if (File.Exists(Path.Combine(_storeRoot, rp)))
                {
                    idx = rp.LastIndexOf('.');
                    // add GUID - make it really unique
                    rp = rp.Substring(0, idx - 1)
                        + "_"
                        + Guid.NewGuid().ToString("N")
                        + rp.Substring(idx);
                }
            }

            // save file
            //
            string path = Path.GetDirectoryName(Path.Combine(_storeRoot, rp));
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            
            using (FileStream fs = File.Create(Path.Combine(_storeRoot, rp))) data.CopyTo(fs);

            return rp;
        }

        public void Dispose() { }

        /// <summary>
        /// Removes unwanted characters from path element to conform to file system requirements; may result in empty string returned.
        /// </summary>
        /// <param name="element">String to sanitize</param>
        /// <param name="charsToClear">Characters to remove from string</param>
        protected static void sanitizePathElement(ref string element, char[] charsToClear)
        {
            int idx;
            do
            {
                idx = element.IndexOfAny(charsToClear);
                if (idx >= 0) element.Remove(idx, 1);
            }
            while (idx >= 0);
        }

        /// <summary>
        /// Returns full path to file based on relative path returned previously by <see cref="storeFile"/>
        /// </summary>
        protected string convertToLocalPath(string relativePath)
        {
            return Path.Combine(_storeRoot, relativePath);
        }

        /// <summary>
        /// Removes file from store; path can be either relative or full.
        /// <para>Throws exception if file does not exist</para>
        /// </summary>
        protected void removeFile(string path)
        {
            if (!Path.IsPathRooted(path)) path = convertToLocalPath(path);
            if (!File.Exists(path)) throw new FileNotFoundException("File does not exist", path);
            File.Delete(path);
        }

        public abstract string StoreFile(string namespaceHint, string typeHint, string extension, string idHint, Stream data);

        public abstract string ConvertToFullPath(string relativePath);

        public abstract void RemoveFile(string relativePath);

        public abstract StorageType Type { get; }
    }
}