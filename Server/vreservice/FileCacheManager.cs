using System;
using System.IO;
using System.Collections.Generic;
using System.Security;

namespace Vre.Server
{
    /// <summary>
    /// Maintains cache folder space within a specified limit.
    /// <br/>May temporary exceed limit when new file is added.
    /// <br/>Doues not account for file slack space.
    /// </summary>
    internal class FileCacheManager
    {
        private long _cacheSpaceLimit;
        private long _cacheSpaceStep;
        private string _cacheRoot = null;

        private long _currentSize;
        private List<FileInfo> _files;

        public void Initialize()
        {
            _cacheSpaceLimit = (long)ServiceInstances.Configuration.GetValue("CacheSpaceLimitMb", 10) * 1024L * 1024L;
            _cacheSpaceStep = _cacheSpaceLimit / 10;
            _cacheRoot = ServiceInstances.Configuration.GetValue("CacheRoot", ".");

            try
            {
                if (!Directory.Exists(_cacheRoot)) Directory.CreateDirectory(_cacheRoot);
            }
            catch (Exception ex)
            {
                ServiceInstances.Logger.Error(
                    "Cannot access cache files root ('{0}'); cache management disabled.\r\n{1}",
                    _cacheRoot, ex);
                _cacheRoot = null;
                return;
            }

            // collect current file list
            //
            _files = new List<FileInfo>();
            collectFiles(new DirectoryInfo(_cacheRoot), ref _files);

            // sort files by access time
            //
            _files.Sort((FileInfo x, FileInfo y) => { return x.LastAccessTimeUtc.CompareTo(y.LastAccessTimeUtc); });

            // find if we exceeded defined space limit
            //
            _currentSize = 0;
            for (int idx = _files.Count - 1; idx >= 0; idx--) _currentSize += _files[idx].Length;

            // check we've run out of space limit already
            adjustCacheSize();
        }

        public void AddManagedFile(string filePath)
        {
            if (null == _cacheRoot) return;  // no service

            FileInfo fi = new FileInfo(filePath);
            
            lock (_files)
            {
                _currentSize += fi.Length;
                _files.Add(fi);
                adjustCacheSize();
            }
        }

        private void adjustCacheSize()
        {
            int idx = 0;

            lock (_files)
            {
                while ((_currentSize > _cacheSpaceLimit) && (idx < _files.Count))
                {
                    try
                    {
                        _files[idx].Delete();
                        _currentSize -= _files[idx].Length;
                    }
                    catch (Exception ex)
                    {
                        ServiceInstances.Logger.Error("Cannot delete file '{0}':\r\n{1}", _files[idx].FullName, ex);
                    }
                    idx++;
                }

                _files.RemoveRange(0, idx);
            }
        }

        private static void collectFiles(DirectoryInfo d, ref List<FileInfo> list)
        {
            list.AddRange(d.GetFiles());

            try
            {
                foreach (DirectoryInfo di in d.GetDirectories())
                    collectFiles(di, ref list);
            }
            catch (Exception ex) 
            {
                ServiceInstances.Logger.Error(
                    "Cannot get subdirectories of '{0}'; these folders are excluded from cache management.\r\n{1}",
                    d.FullName, ex);
            }
        }
    }
}