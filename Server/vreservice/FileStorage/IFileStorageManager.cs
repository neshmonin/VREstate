using System;
using System.IO;

namespace Vre.Server.FileStorage
{
    internal enum StorageType
    {
        /// <summary>
        /// Implies file-based acccess
        /// </summary>
        Internal,
        /// <summary>
        /// Implies URL-based access
        /// </summary>
        Public
    }

    internal interface IFileStorageManager : IDisposable
    {
        /// <summary>
        /// Store a new file in storage.
        /// </summary>
        /// <param name="namespaceHint">Namespace hint for name generation, e.g. 'model'; can be empty/null</param>
        /// <param name="typeHint">Type hint for name generation, e.g. 'w' for wireframe; can be empty/null</param>
        /// <param name="extension">File extension; empty/null results in 'bin'</param>
        /// <param name="idHint">Unique identifier hint for name generation; can be empty/null</param>
        /// <returns>Relative path to stored file; to be concatenated with <see cref="Root"/></returns>
        string StoreFile(string namespaceHint, string typeHint, string extension, string idHint, Stream data);

        /// <summary>
        /// Returns full path to the file based on relative path returned by <see cref="StoreFile"/>
        /// </summary>
        string ConvertToFullPath(string relativePath);

        /// <summary>
        /// Removes stored file based on relative path returned by <see cref="StoreFile"/>
        /// </summary>
        void RemoveFile(string relativePath);

        /// <summary>
        /// Type of storage
        /// </summary>
        StorageType Type { get; }

        Stream OpenFile(string relativePath);
    }
}
