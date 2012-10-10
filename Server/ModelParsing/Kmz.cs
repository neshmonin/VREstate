using System;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace VrEstate
{
    /// <summary>
    /// Unique class for reading/writing KMZ files.
    /// </summary>
    public class Kmz : IDisposable
    {
        /// <summary>
        /// Compression method enumeration
        /// </summary>
        public enum Compression : ushort { 
            /// <summary>Uncompressed storage</summary> 
            Store = 0, 
            /// <summary>Deflate compression method</summary>
            Deflate = 8 }

        /// <summary>
        /// Represents an entry in Kmz file directory
        /// </summary>
        public struct KmzFileEntry
        {
            /// <summary>Compression method</summary>
            public Compression Method; 
            /// <summary>Full path and filename as stored in Kmz</summary>
            public string FilenameInKmz;
            /// <summary>Original file size</summary>
            public uint FileSize;
            /// <summary>Compressed file size</summary>
            public uint CompressedSize;
            /// <summary>Offset of header information inside Kmz storage</summary>
            public uint HeaderOffset;
            /// <summary>Offset of file inside Kmz storage</summary>
            public uint FileOffset;
            /// <summary>Size of header information</summary>
            public uint HeaderSize;
            /// <summary>32-bit checksum of entire file</summary>
            public uint Crc32;
            /// <summary>Last modification time of file</summary>
            public DateTime ModifyTime;
            /// <summary>User comment for file</summary>
            public string Comment;
            /// <summary>True if UTF8 encoding for filename and comments, false if default (CP 437)</summary>
            public bool EncodeUTF8;

            /// <summary>Overriden method</summary>
            /// <returns>Filename in Kmz</returns>
            public override string ToString()
            {
                return this.FilenameInKmz;
            }
        }

        public class UpdatableItem
        {
            public Kmz.KmzFileEntry fileEntry;
            public Stream stream;

            public UpdatableItem(Kmz.KmzFileEntry fileEntry, Stream stream)
            {
                this.fileEntry = fileEntry;
                this.stream = stream;
            }
        }

        #region Public fields
        /// <summary>True if UTF8 encoding for filename and comments, false if default (CP 437)</summary>
        public bool EncodeUTF8 = false;
        /// <summary>Force deflate algotithm even if it inflates the stored file. Off by default.</summary>
        public bool ForceDeflating = false;

        public static string m_versionInfoFileName = "versionInfo";
        public static string SiteName
        {
            get { return Kmz.m_siteName; }
            set { Kmz.m_siteName = value; }
        }
        public static string DeveloperName
        {
            get { return Kmz.m_developerName; }
            set { Kmz.m_developerName = value; }
        }
        #endregion

        #region Private fields
        // List of files to store
        private List<KmzFileEntry> Files = new List<KmzFileEntry>();
        // Filename of storage file
        private string FileName;
        // Stream object of storage file
        private Stream KmzFileStream;
        // General comment
        private string Comment = "";
        // Central dir image
        private byte[] CentralDirImage = null;
        // Existing files in kmz
        private ushort ExistingFiles = 0;
        // File access for Open method
        private FileAccess Access;
        // Static CRC32 Table
        private static UInt32[] CrcTable = null;
        // Default filename encoder
        private static Encoding DefaultEncoding = Encoding.GetEncoding(437);

        private static long m_lastKmzVersion = 0;

        private UpdatableItem m_kml = null;
        private UpdatableItem m_collada = null;
        private int m_updateNum = 1;

        private static string m_backupKmzName = string.Empty;
        private static string m_siteName;
        private static string m_developerName;

        #endregion

        #region Public methods
        // Static constructor. Just invoked once in order to create the CRC32 lookup table.
        static Kmz()
        {
            // Generate CRC32 table
            CrcTable = new UInt32[256];
            for (int i = 0; i < CrcTable.Length; i++)
            {
                UInt32 c = (UInt32)i;
                for (int j = 0; j < 8; j++)
                {
                    if ((c & 1) != 0)
                        c = 3988292384 ^ (c >> 1);
                    else
                        c >>= 1;
                }
                CrcTable[i] = c;
            }

        }
        /// <summary>
        /// Method to create a new storage file
        /// </summary>
        /// <param name="_filename">Full path of Kmz file to create</param>
        /// <param name="_comment">General comment for Kmz file</param>
        /// <returns>A valid Kmz object</returns>
        public static Kmz Create(string _filename, string _comment)
        {
            Stream stream = new FileStream(_filename, FileMode.Create, FileAccess.ReadWrite);

            Kmz kmz = Create(stream, _comment);
            kmz.FileName = _filename;
            return kmz;
        }
        /// <summary>
        /// Method to create a new kmz storage in a stream
        /// </summary>
        /// <param name="_stream"></param>
        /// <param name="_comment"></param>
        /// <returns>A valid Kmz object</returns>
        public static Kmz Create(Stream _stream, string _comment)
        {
            Kmz kmz = new Kmz();
            kmz.Comment = _comment;
            kmz.KmzFileStream = _stream;
            kmz.Access = FileAccess.Write;
            return kmz;
        }

        /// <summary>
        /// Method to open an existing storage file
        /// </summary>
        /// <param name="_filename">Full path of Kmz file to open</param>
        /// <param name="_access">File access mode as used in FileStream constructor</param>
        /// <returns>A valid Kmz object</returns>
        public static Kmz Open(string filename, FileAccess access)
        {
            m_lastKmzVersion = GetLastKmzVersion();
            string filePath = GetLocalSitePath();
            string fullFileName = Path.Combine(filePath, filename);

            if (Path.IsPathRooted(filename))
            {
                fullFileName = filename;
                filePath = Path.GetDirectoryName(filename);
            }

            Stream stream = (Stream)new FileStream(fullFileName, FileMode.Open, access);

            Kmz kmz = Open(stream, access);
            kmz.FileName = fullFileName;

            string[] kmzFiles = Directory.GetFiles(filePath, "*.kmz");
            foreach (string file in kmzFiles)
            {
                if (file.Contains(Path.GetFileNameWithoutExtension(kmz.FileName) + "(update "))
                    kmz.m_updateNum ++;
            }

            return kmz;
        }

        /// <summary>
        /// Method to open kmz from Url
        /// </summary>
        /// <param name="uri">Uri of the kmz</param>
        /// <returns>A valid Kmz object</returns>
        public static Kmz Open(Uri uri)
        {
            WebRequest request = Kmz.CreateHttpRequest(uri);
            Stream input = request.GetResponse().GetResponseStream();
            MemoryStream memStream = new MemoryStream();
            CopyStream(input, memStream);

            Kmz kmz = Open(memStream, FileAccess.Read);
            kmz.FileName = uri.LocalPath;
            return kmz;
        }

        public XmlDocument GetKmlDoc()
        {
            m_kml = extractUpdatableItem("doc.kml");

            m_kml.stream.Seek(0, SeekOrigin.Begin);
            XmlDocument doc = new XmlDocument();
            doc.Load(m_kml.stream);

            return doc;
        }

        public XmlDocument GetColladaDoc()
        {
            m_collada = extractUpdatableItem("untitled.dae");
            if (m_collada == null)
                m_collada = extractUpdatableItem("untitled_98.dae");

            m_collada.stream.Seek(0, SeekOrigin.Begin);
            XmlDocument doc = new XmlDocument();
            doc.Load(m_collada.stream);

            return doc;
        }

        public void SaveColladaDoc(XmlDocument doc)
        {
            m_collada.stream = new MemoryStream();
            doc.Save(m_collada.stream);
        }

        /// <summary>
        /// Method to open an existing storage from stream
        /// </summary>
        /// <param name="_stream">Already opened stream with kmz contents</param>
        /// <param name="_access">File access mode for stream operations</param>
        /// <returns>A valid Kmz object</returns>
        public static Kmz Open(Stream _stream, FileAccess _access)
        {
            Stream inpStream = _stream;

            if (!_stream.CanSeek)
            {
                // make a local copy of the file and then use MemoryStream to read it:
            }

            Kmz kmz = new Kmz();
            //kmz.FileName = _filename;
            kmz.KmzFileStream = inpStream;
            kmz.Access = _access;

            if (kmz.ReadFileInfo())
                return kmz;

            throw new System.IO.InvalidDataException();
        }
        /// <summary>
        /// Add full contents of a file into the Kmz storage
        /// </summary>
        /// <param name="_method">Compression method</param>
        /// <param name="_pathname">Full path of file to add to Kmz storage</param>
        /// <param name="_filenameInKmz">Filename and path as desired in Kmz directory</param>
        /// <param name="_comment">Comment for stored file</param>        
        public void AddFile(Compression _method, string _pathname, string _filenameInKmz, string _comment)
        {
            if (Access == FileAccess.Read)
                throw new InvalidOperationException("Writing is not alowed");

            FileStream stream = new FileStream(_pathname, FileMode.Open, FileAccess.Read);
            AddStream(_method, _filenameInKmz, stream, File.GetLastWriteTime(_pathname), _comment);
            stream.Close();
        }
        /// <summary>
        /// Add full contents of a stream into the Kmz storage
        /// </summary>
        /// <param name="_method">Compression method</param>
        /// <param name="_filenameInKmz">Filename and path as desired in Kmz directory</param>
        /// <param name="_source">Stream object containing the data to store in Kmz</param>
        /// <param name="_modTime">Modification time of the data to store</param>
        /// <param name="_comment">Comment for stored file</param>
        public void AddStream(Compression _method, string _filenameInKmz, Stream _source, DateTime _modTime, string _comment)
        {
            if (Access == FileAccess.Read)
                throw new InvalidOperationException("Writing is not alowed");

            long offset;
            if (this.Files.Count==0)
                offset = 0;
            else
            {
                KmzFileEntry last = this.Files[this.Files.Count-1];
                offset = last.HeaderOffset + last.HeaderSize;
            }

            // Prepare the fileinfo
            KmzFileEntry zfe = new KmzFileEntry();
            zfe.Method = _method;
            zfe.EncodeUTF8 = this.EncodeUTF8;
            zfe.FilenameInKmz = NormalizedFilename(_filenameInKmz);
            zfe.Comment = (_comment == null ? "" : _comment);

            // Even though we write the header now, it will have to be rewritten, since we don't know compressed size or crc.
            zfe.Crc32 = 0;  // to be updated later
            zfe.HeaderOffset = (uint)this.KmzFileStream.Position;  // offset within file of the start of this local record
            zfe.ModifyTime = _modTime;

            // Write local header
            WriteLocalHeader(ref zfe);
            zfe.FileOffset = (uint)this.KmzFileStream.Position;

            // Write file to kmz (store)
            Store(ref zfe, _source);
            _source.Close();

            this.UpdateCrcAndSizes(ref zfe);

            Files.Add(zfe);
        }
        /// <summary>
        /// Updates central directory (if pertinent) and close the Kmz storage
        /// </summary>
        /// <remarks>This is a required step, unless automatic dispose is used</remarks>
        public void Close()
        {
            if (this.KmzFileStream == null)
                return;

            if (this.Access != FileAccess.Read)
            {
                if (m_kml != null)
                {
                    m_kml.stream.Flush();
                    m_kml.stream.Dispose();
                    m_kml = null;
                }

                if (m_collada != null)
                {
                    m_collada.stream.Flush();
                    m_collada.stream.Dispose();
                    m_collada = null;
                }

                uint centralOffset = (uint)this.KmzFileStream.Position;
                uint centralSize = 0;

                if (this.CentralDirImage != null)
                    this.KmzFileStream.Write(CentralDirImage, 0, CentralDirImage.Length);

                for (int i = 0; i < Files.Count; i++)
                {
                    KmzFileEntry entry = Files[i];
                    long pos = this.KmzFileStream.Position;
                    this.WriteCentralDirRecord(entry);
                    centralSize += (uint)(this.KmzFileStream.Position - pos);
                }

                if (this.CentralDirImage != null)
                    this.WriteEndRecord(centralSize + (uint)CentralDirImage.Length, centralOffset);
                else
                    this.WriteEndRecord(centralSize, centralOffset);
            }

            if (this.KmzFileStream != null)
            {
                this.KmzFileStream.Flush();
                this.KmzFileStream.Dispose();
                this.KmzFileStream = null;
            }
        }
        /// <summary>
        /// Read all the file records in the central directory 
        /// </summary>
        /// <returns>List of all entries in directory</returns>
        public List<KmzFileEntry> ReadCentralDir()
        {
            if (this.CentralDirImage == null)
                throw new InvalidOperationException("Central directory currently does not exist");

            List<KmzFileEntry> result = new List<KmzFileEntry>();

            for (int pointer = 0; pointer < this.CentralDirImage.Length; )
            {
                uint signature = BitConverter.ToUInt32(CentralDirImage, pointer);
                if (signature != 0x02014b50)
                    break;

                bool encodeUTF8 = (BitConverter.ToUInt16(CentralDirImage, pointer + 8) & 0x0800) != 0;
                ushort method = BitConverter.ToUInt16(CentralDirImage, pointer + 10);
                uint modifyTime = BitConverter.ToUInt32(CentralDirImage, pointer + 12);
                uint crc32 = BitConverter.ToUInt32(CentralDirImage, pointer + 16);
                uint comprSize = BitConverter.ToUInt32(CentralDirImage, pointer + 20);
                uint fileSize = BitConverter.ToUInt32(CentralDirImage, pointer + 24);
                ushort filenameSize = BitConverter.ToUInt16(CentralDirImage, pointer + 28);
                ushort extraSize = BitConverter.ToUInt16(CentralDirImage, pointer + 30);
                ushort commentSize = BitConverter.ToUInt16(CentralDirImage, pointer + 32);
                uint headerOffset = BitConverter.ToUInt32(CentralDirImage, pointer + 42);
                uint headerSize = (uint)( 46 + filenameSize + extraSize + commentSize);

                Encoding encoder = encodeUTF8 ? Encoding.UTF8 : DefaultEncoding;

                KmzFileEntry zfe = new KmzFileEntry();
                zfe.Method = (Compression)method;
                zfe.FilenameInKmz = encoder.GetString(CentralDirImage, pointer + 46, filenameSize);
                zfe.FileOffset = GetFileOffset(headerOffset);
                zfe.FileSize = fileSize;
                zfe.CompressedSize = comprSize;
                zfe.HeaderOffset = headerOffset;
                zfe.HeaderSize = headerSize;
                zfe.Crc32 = crc32;
                zfe.ModifyTime = DosTimeToDateTime(modifyTime);
                if (commentSize > 0)
                    zfe.Comment = encoder.GetString(CentralDirImage, pointer + 46 + filenameSize + extraSize, commentSize);

                result.Add(zfe);
                pointer += (46 + filenameSize + extraSize + commentSize);
            }

            return result;
        }

        public bool ExtractFile(UpdatableItem item)
        {
            return ExtractFile(item.fileEntry, item.stream);
        }

        /// <summary>
        /// Copy the contents of a stored file into a physical file
        /// </summary>
        /// <param name="_zfe">Entry information of file to extract</param>
        /// <param name="_filename">Name of file to store uncompressed data</param>
        /// <returns>True if success, false if not.</returns>
        /// <remarks>Unique compression methods are Store and Deflate</remarks>
        public bool ExtractFile(KmzFileEntry _zfe, string _filename)
        {
            // Make sure the parent directory exist
            string path = System.IO.Path.GetDirectoryName(_filename);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            // Check it is directory. If so, do nothing
            if (Directory.Exists(_filename))
                return true;

            Stream output = new FileStream(_filename, FileMode.Create, FileAccess.Write);
            bool result = ExtractFile(_zfe, output);
            if (result)
                output.Close();

            //File.SetCreationTime(_filename, _zfe.ModifyTime);
            //File.SetLastWriteTime(_filename, _zfe.ModifyTime);
            
            return result;
        }
        /// <summary>
        /// Copy the contents of a stored file into an opened stream
        /// </summary>
        /// <param name="_zfe">Entry information of file to extract</param>
        /// <param name="_stream">Stream to store the uncompressed data</param>
        /// <returns>True if success, false if not.</returns>
        /// <remarks>Unique compression methods are Store and Deflate</remarks>
        public bool ExtractFile(KmzFileEntry _zfe, Stream _stream)
        {
            if (!_stream.CanWrite)
                throw new InvalidOperationException("Stream cannot be written");

            // check signature
            byte[] signature = new byte[4];
            this.KmzFileStream.Seek(_zfe.HeaderOffset, SeekOrigin.Begin);
            this.KmzFileStream.Read(signature, 0, 4);
            if (BitConverter.ToUInt32(signature, 0) != 0x04034b50)
                return false;

            // Select input stream for inflating or just reading
            Stream inStream;
            if (_zfe.Method == Compression.Store)
                inStream = this.KmzFileStream;
            else if (_zfe.Method == Compression.Deflate)
                inStream = new DeflateStream(this.KmzFileStream, CompressionMode.Decompress, true);
            else
                return false;

            // Buffered copy
            byte[] buffer = new byte[16384];
            this.KmzFileStream.Seek(_zfe.FileOffset, SeekOrigin.Begin);
            uint bytesPending = _zfe.FileSize;
            while (bytesPending > 0)
            {
                int bytesRead = inStream.Read(buffer, 0, (int)Math.Min(bytesPending, buffer.Length));
                if (bytesRead == 0)
                    break;

                _stream.Write(buffer, 0, bytesRead);
                bytesPending -= (uint)bytesRead;
            }
            _stream.Flush();

            if (_zfe.Method == Compression.Deflate)
                inStream.Dispose();
            return true;
        }

        public static string GetWebrootUrl(string fileName)
        {
            string webAlias = string.Empty;
            if (!string.IsNullOrEmpty(m_WebRootAlias))
                webAlias = m_WebRootAlias.TrimEnd('/').TrimStart('/');

            if (!string.IsNullOrEmpty(webAlias))
                webAlias = webAlias + "/";

            string fullWebAddress = string.Format(@"http://{0}:{1}/{2}SuperServer/{3}",
                                                  m_httpServerUrl,
                                                  m_httpServerPort,
                                                  webAlias,
                                                  fileName);
            fullWebAddress.TrimEnd('/');
            return fullWebAddress;
        }

        public static string GetSiteUrl(string fileName)
        {
            string webAlias = string.Empty;
            if (!string.IsNullOrEmpty(m_WebRootAlias))
                webAlias = m_WebRootAlias.TrimEnd('/').TrimStart('/');

            if (!string.IsNullOrEmpty(webAlias))
                webAlias = webAlias + "/";

            string fullWebAddress = string.Format(@"http://{0}:{1}/{2}SuperServer/{3}/{4}/{5}",
                                                  m_httpServerUrl,
                                                  m_httpServerPort,
                                                  webAlias,
                                                  m_developerName,
                                                  m_siteName,
                                                  fileName);
            fullWebAddress.TrimEnd('/');
            return fullWebAddress;
        }

        public static string GetCommonResourcesUrl(string fileName)
        {
            string webAlias = string.Empty;
            if (!string.IsNullOrEmpty(m_WebRootAlias))
                webAlias = m_WebRootAlias.TrimEnd('/').TrimStart('/');

            if (!string.IsNullOrEmpty(webAlias))
                webAlias = webAlias + "/";

            string fullWebAddress = string.Format(@"http://{0}:{1}/{2}{3}/{4}",
                                                  m_httpServerUrl,
                                                  m_httpServerPort,
                                                  webAlias,
                                                  "VrEstateLoadable",
                                                  fileName);
            fullWebAddress.TrimEnd('/');
            return fullWebAddress;
        }

        public static string GetSiteCachedUrl(string fileName)
        {
            string webAlias = string.Empty;
            if (!string.IsNullOrEmpty(m_WebRootAlias))
                webAlias = m_WebRootAlias.TrimEnd('/').TrimStart('/');

            if (!string.IsNullOrEmpty(webAlias))
                webAlias = webAlias + "/";

            string fullWebAddress = string.Format(@"http://{0}:{1}/{2}SuperServer/{3}/{4}/{5}/{6}",
                                                  m_httpServerUrl,
                                                  m_httpServerPort,
                                                  webAlias,
                                                  m_developerName,
                                                  m_siteName,
                                                  "Cached",
                                                  fileName);
            fullWebAddress.TrimEnd('/');
            return fullWebAddress;
        }

        public static string GetLocalSitePath()
        {
            string filePath = string.Format(@"{0}\SuperServer\{1}\{2}",
                                            SuperServer.WebRootLocalPath,
                                            m_developerName,
                                            m_siteName);
            return filePath;
        }

        /// <remarks>This method only works for storage of type FileStream</remarks>
        public static Kmz UpdateAndBackupKmz(Kmz _kmz)
        {
            if (!(_kmz.KmzFileStream is FileStream))
                throw new InvalidOperationException("RemoveEntries is allowed just over streams of type FileStream");

            //Get full list of entries
            List<KmzFileEntry> fullList = _kmz.ReadCentralDir();

            //In order to delete we need to create a copy of the kmz file excluding the selected items
            string tempKmzName = Path.GetTempFileName();
            string tempEntryName = Path.GetTempFileName();
            Kmz tempKmz = null;

            try
            {
                tempKmz = Kmz.Create(tempKmzName, string.Empty);

                foreach (KmzFileEntry zfe in fullList)
                {
                    if (_kmz.m_collada.fileEntry.FilenameInKmz != zfe.FilenameInKmz)
                    {
                        if (_kmz.ExtractFile(zfe, tempEntryName))
                        {
                            tempKmz.AddFile(zfe.Method, tempEntryName, zfe.FilenameInKmz, zfe.Comment);
                        }
                    }
                }
                if (_kmz.m_collada != null)
                {
                    _kmz.m_collada.stream.Position = 0;
                    tempKmz.AddStream(_kmz.m_collada.fileEntry.Method,
                                      _kmz.m_collada.fileEntry.FilenameInKmz,
                                      _kmz.m_collada.stream,
                                      DateTime.Now, string.Empty);
                }

                _kmz.Close();
                tempKmz.Close();

                string newName = string.Format("{0}(update {1}){2}",
                    Path.GetFileNameWithoutExtension(_kmz.FileName),
                    _kmz.m_updateNum.ToString("D3"),
                    Path.GetExtension(_kmz.FileName));

                string backupKmzName = Path.Combine(Path.GetDirectoryName(_kmz.FileName), newName);
                // rename
                File.Move(_kmz.FileName, backupKmzName);
                File.Move(tempKmzName, _kmz.FileName);

                // update also the version info
                m_lastKmzVersion = GetLastKmzVersion() + 1;
                string verInfoFile = Path.Combine(Path.GetDirectoryName(_kmz.FileName), m_versionInfoFileName);
                FileStream verInfo;
                if (File.Exists(verInfoFile))
                    verInfo = File.Open(verInfoFile, FileMode.Truncate, FileAccess.Write);
                else
                    verInfo = File.Open(verInfoFile, FileMode.Create, FileAccess.Write);

                using (StreamWriter writeFile = new StreamWriter(verInfo))
                {
                    writeFile.WriteLine(m_lastKmzVersion.ToString());
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                if (File.Exists(tempKmzName))
                    File.Delete(tempEntryName);
                if (File.Exists(tempEntryName))
                    File.Delete(tempEntryName);
            }
            return tempKmz;
        }

        public static bool VersionChanged(out long newVersion)
        {
            newVersion = GetLastKmzVersion();
            bool changed = newVersion != m_lastKmzVersion;
            m_lastKmzVersion = newVersion;
            return changed;
        }

        public static MemoryStream FetchFromSiteUrl(string fileName)
        {
            try
            {
                string fullWebAddress = GetSiteUrl(fileName);
                WebRequest request = Kmz.CreateHttpRequest(new Uri(fullWebAddress));
                Stream input = request.GetResponse().GetResponseStream();
                MemoryStream memStream = new MemoryStream();
                CopyStream(input, memStream);
                input.Close();
                memStream.Position = 0;
                return memStream;
            }
            catch (System.Net.WebException) { }
            return null;
        }

        public static long GetLastKmzVersion()
        {
            MemoryStream memStream = FetchFromSiteUrl(m_versionInfoFileName);
            if (memStream == null)
                return 0;

            long lastKmzVersion = 0;
            using (StreamReader SR = new StreamReader(memStream))
            {
                string line = SR.ReadLine();
                lastKmzVersion = long.Parse(line);
            }

            return lastKmzVersion;
        }

        private static string m_WebRootAlias = "";

        public static string WebRootAlias
        {
            get { return Kmz.m_WebRootAlias; }
            set { Kmz.m_WebRootAlias = value; }
        }
        private static string m_httpServerUrl = "localhost";
        public static string HttpServerUrl
        {
            get { return m_httpServerUrl; }
            set { m_httpServerUrl = value; }
        }

        private static int m_httpServerPort = 80;

        public static int HttpServerPort
        {
            get { return m_httpServerPort; }
            set { m_httpServerPort = value; }
        }
        #endregion

        #region Private methods

        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[0x1000];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                output.Write(buffer, 0, read);
        }

        private static HttpWebRequest CreateHttpRequest(Uri uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.AllowAutoRedirect = true;
            if (!string.IsNullOrEmpty(HttpServerUrl))
            {
                var proxy = new WebProxy(HttpServerUrl, HttpServerPort);
                proxy.BypassProxyOnLocal = true;
                proxy.Credentials = CredentialCache.DefaultCredentials;
                request.Proxy = proxy;
            }
            return request;
        }

        // Calculate the file offset by reading the corresponding local header
        private uint GetFileOffset(uint _headerOffset)
        {
            byte[] buffer = new byte[2];

            this.KmzFileStream.Seek(_headerOffset + 26, SeekOrigin.Begin);
            this.KmzFileStream.Read(buffer, 0, 2);
            ushort filenameSize = BitConverter.ToUInt16(buffer, 0);
            this.KmzFileStream.Read(buffer, 0, 2);
            ushort extraSize = BitConverter.ToUInt16(buffer, 0);

            return (uint)(30 + filenameSize + extraSize + _headerOffset);
        }
        /* Local file header:
            local file header signature     4 bytes  (0x04034b50)
            version needed to extract       2 bytes
            general purpose bit flag        2 bytes
            compression method              2 bytes
            last mod file time              2 bytes
            last mod file date              2 bytes
            crc-32                          4 bytes
            compressed size                 4 bytes
            uncompressed size               4 bytes
            filename length                 2 bytes
            extra field length              2 bytes

            filename (variable size)
            extra field (variable size)
        */
        private void WriteLocalHeader(ref KmzFileEntry _zfe)
        {
            long pos = this.KmzFileStream.Position;
            Encoding encoder = _zfe.EncodeUTF8 ? Encoding.UTF8 : DefaultEncoding;
            byte[] encodedFilename = encoder.GetBytes(_zfe.FilenameInKmz);

            this.KmzFileStream.Write(new byte[] { 80, 75, 3, 4, 20, 0}, 0, 6); // No extra header
            this.KmzFileStream.Write(BitConverter.GetBytes((ushort)(_zfe.EncodeUTF8 ? 0x0800 : 0)), 0, 2); // filename and comment encoding 
            this.KmzFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 2);  // kmzping method
            this.KmzFileStream.Write(BitConverter.GetBytes(DateTimeToDosTime(_zfe.ModifyTime)), 0, 4); // kmzping date and time
            this.KmzFileStream.Write(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0, 12); // unused CRC, un/compressed size, updated later
            this.KmzFileStream.Write(BitConverter.GetBytes((ushort)encodedFilename.Length), 0, 2); // filename length
            this.KmzFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2); // extra length

            this.KmzFileStream.Write(encodedFilename, 0, encodedFilename.Length);
            _zfe.HeaderSize = (uint)(this.KmzFileStream.Position - pos);
        }
        /* Central directory's File header:
            central file header signature   4 bytes  (0x02014b50)
            version made by                 2 bytes
            version needed to extract       2 bytes
            general purpose bit flag        2 bytes
            compression method              2 bytes
            last mod file time              2 bytes
            last mod file date              2 bytes
            crc-32                          4 bytes
            compressed size                 4 bytes
            uncompressed size               4 bytes
            filename length                 2 bytes
            extra field length              2 bytes
            file comment length             2 bytes
            disk number start               2 bytes
            internal file attributes        2 bytes
            external file attributes        4 bytes
            relative offset of local header 4 bytes

            filename (variable size)
            extra field (variable size)
            file comment (variable size)
        */
        private void WriteCentralDirRecord(KmzFileEntry _zfe)
        {
            Encoding encoder = _zfe.EncodeUTF8 ? Encoding.UTF8 : DefaultEncoding;
            byte[] encodedFilename = encoder.GetBytes(_zfe.FilenameInKmz);
            byte[] encodedComment = encoder.GetBytes(_zfe.Comment);

            this.KmzFileStream.Write(new byte[] { 80, 75, 1, 2, 23, 0xB, 20, 0 }, 0, 8);
            this.KmzFileStream.Write(BitConverter.GetBytes((ushort)(_zfe.EncodeUTF8 ? 0x0800 : 0)), 0, 2); // filename and comment encoding 
            this.KmzFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 2);  // kmzping method
            this.KmzFileStream.Write(BitConverter.GetBytes(DateTimeToDosTime(_zfe.ModifyTime)), 0, 4);  // kmzping date and time
            this.KmzFileStream.Write(BitConverter.GetBytes(_zfe.Crc32), 0, 4); // file CRC
            this.KmzFileStream.Write(BitConverter.GetBytes(_zfe.CompressedSize), 0, 4); // compressed file size
            this.KmzFileStream.Write(BitConverter.GetBytes(_zfe.FileSize), 0, 4); // uncompressed file size
            this.KmzFileStream.Write(BitConverter.GetBytes((ushort)encodedFilename.Length), 0, 2); // Filename in kmz
            this.KmzFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2); // extra length
            this.KmzFileStream.Write(BitConverter.GetBytes((ushort)encodedComment.Length), 0, 2);

            this.KmzFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2); // disk=0
            this.KmzFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2); // file type: binary
            this.KmzFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2); // Internal file attributes
            this.KmzFileStream.Write(BitConverter.GetBytes((ushort)0x8100), 0, 2); // External file attributes (normal/readable)
            this.KmzFileStream.Write(BitConverter.GetBytes(_zfe.HeaderOffset), 0, 4);  // Offset of header

            this.KmzFileStream.Write(encodedFilename, 0, encodedFilename.Length);
            this.KmzFileStream.Write(encodedComment, 0, encodedComment.Length);
        }
        /* End of central dir record:
            end of central dir signature    4 bytes  (0x06054b50)
            number of this disk             2 bytes
            number of the disk with the
            start of the central directory  2 bytes
            total number of entries in
            the central dir on this disk    2 bytes
            total number of entries in
            the central dir                 2 bytes
            size of the central directory   4 bytes
            offset of start of central
            directory with respect to
            the starting disk number        4 bytes
            kmzfile comment length          2 bytes
            kmzfile comment (variable size)
        */
        private void WriteEndRecord(uint _size, uint _offset)
        {
            Encoding encoder = this.EncodeUTF8 ? Encoding.UTF8 : DefaultEncoding;
            byte[] encodedComment = encoder.GetBytes(this.Comment);

            this.KmzFileStream.Write(new byte[] { 80, 75, 5, 6, 0, 0, 0, 0 }, 0, 8);
            this.KmzFileStream.Write(BitConverter.GetBytes((ushort)Files.Count+ExistingFiles), 0, 2);
            this.KmzFileStream.Write(BitConverter.GetBytes((ushort)Files.Count+ExistingFiles), 0, 2);
            this.KmzFileStream.Write(BitConverter.GetBytes(_size), 0, 4);
            this.KmzFileStream.Write(BitConverter.GetBytes(_offset), 0, 4);
            this.KmzFileStream.Write(BitConverter.GetBytes((ushort)encodedComment.Length), 0, 2);
            this.KmzFileStream.Write(encodedComment, 0, encodedComment.Length);
        }
        // Copies all source file into storage file
        private void Store(ref KmzFileEntry _zfe, Stream _source)
        {
            byte[] buffer = new byte[16384];
            int bytesRead;
            uint totalRead = 0;
            Stream outStream;

            long posStart = this.KmzFileStream.Position;
            long sourceStart = _source.Position;

            if (_zfe.Method == Compression.Store)
                outStream = this.KmzFileStream;
            else
                outStream = new DeflateStream(this.KmzFileStream, CompressionMode.Compress, true);

            _zfe.Crc32 = 0 ^ 0xffffffff;
            
            do
            {
                bytesRead = _source.Read(buffer, 0, buffer.Length);
                totalRead += (uint)bytesRead;
                if (bytesRead > 0)
                {
                    outStream.Write(buffer, 0, bytesRead);

                    for (uint i = 0; i < bytesRead; i++)
                    {
                        _zfe.Crc32 = Kmz.CrcTable[(_zfe.Crc32 ^ buffer[i]) & 0xFF] ^ (_zfe.Crc32 >> 8);
                    }
                }
            } while (bytesRead == buffer.Length);
            outStream.Flush();

            if (_zfe.Method == Compression.Deflate)
                outStream.Dispose();

            _zfe.Crc32 ^= 0xffffffff;
            _zfe.FileSize = totalRead;
            _zfe.CompressedSize = (uint)(this.KmzFileStream.Position - posStart);

            // Verify for real compression
            if (_zfe.Method == Compression.Deflate && !this.ForceDeflating && _source.CanSeek && _zfe.CompressedSize > _zfe.FileSize)
            {
                // Start operation again with Store algorithm
                _zfe.Method = Compression.Store;
                this.KmzFileStream.Position = posStart;
                this.KmzFileStream.SetLength(posStart);
                _source.Position = sourceStart;
                this.Store(ref _zfe, _source);
            }
        }
        /* DOS Date and time:
            MS-DOS date. The date is a packed value with the following format. Bits Description 
                0-4 Day of the month (1–31) 
                5-8 Month (1 = January, 2 = February, and so on) 
                9-15 Year offset from 1980 (add 1980 to get actual year) 
            MS-DOS time. The time is a packed value with the following format. Bits Description 
                0-4 Second divided by 2 
                5-10 Minute (0–59) 
                11-15 Hour (0–23 on a 24-hour clock) 
        */
        private uint DateTimeToDosTime(DateTime _dt)
        {
            return (uint)(
                (_dt.Second / 2) | (_dt.Minute << 5) | (_dt.Hour << 11) | 
                (_dt.Day<<16) | (_dt.Month << 21) | ((_dt.Year - 1980) << 25));
        }
        private DateTime DosTimeToDateTime(uint _dt)
        {
            if (_dt == 0)
                return new DateTime();

            return new DateTime(
                (int)(_dt >> 25) + 1980,
                (int)(_dt >> 21) & 15,
                (int)(_dt >> 16) & 31,
                (int)(_dt >> 11) & 31,
                (int)(_dt >> 5) & 63,
                (int)(_dt & 31) * 2);
        }

        /* CRC32 algorithm
          The 'magic number' for the CRC is 0xdebb20e3.  
          The proper CRC pre and post conditioning
          is used, meaning that the CRC register is
          pre-conditioned with all ones (a starting value
          of 0xffffffff) and the value is post-conditioned by
          taking the one's complement of the CRC residual.
          If bit 3 of the general purpose flag is set, this
          field is set to zero in the local header and the correct
          value is put in the data descriptor and in the central
          directory.
        */
        private void UpdateCrcAndSizes(ref KmzFileEntry _zfe)
        {
            long lastPos = this.KmzFileStream.Position;  // remember position

            this.KmzFileStream.Position = _zfe.HeaderOffset + 8;
            this.KmzFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 2);  // kmzping method

            this.KmzFileStream.Position = _zfe.HeaderOffset + 14;
            this.KmzFileStream.Write(BitConverter.GetBytes(_zfe.Crc32), 0, 4);  // Update CRC
            this.KmzFileStream.Write(BitConverter.GetBytes(_zfe.CompressedSize), 0, 4);  // Compressed size
            this.KmzFileStream.Write(BitConverter.GetBytes(_zfe.FileSize), 0, 4);  // Uncompressed size

            this.KmzFileStream.Position = lastPos;  // restore position
        }
        // Replaces backslashes with slashes to store in kmz header
        private string NormalizedFilename(string _filename)
        {
            string filename = _filename.Replace('\\', '/');

            int pos = filename.IndexOf(':');
            if (pos >= 0)
                filename = filename.Remove(0, pos + 1);

            return filename.Trim('/');
        }
        // Reads the end-of-central-directory record
        private bool ReadFileInfo()
        {
            if (this.KmzFileStream.Length < 22)
                return false;

            try
            {
                this.KmzFileStream.Seek(-17, SeekOrigin.End);
                BinaryReader br = new BinaryReader(this.KmzFileStream);
                do
                {
                    this.KmzFileStream.Seek(-5, SeekOrigin.Current);
                    UInt32 sig = br.ReadUInt32();
                    if (sig == 0x06054b50)
                    {
                        this.KmzFileStream.Seek(6, SeekOrigin.Current);

                        UInt16 entries = br.ReadUInt16();
                        Int32 centralSize = br.ReadInt32();
                        UInt32 centralDirOffset = br.ReadUInt32();
                        UInt16 commentSize = br.ReadUInt16();

                        // check if comment field is the very last data in file
                        if (this.KmzFileStream.Position + commentSize != this.KmzFileStream.Length)
                            return false;

                        // Copy entire central directory to a memory buffer
                        this.ExistingFiles = entries;
                        this.CentralDirImage = new byte[centralSize];
                        this.KmzFileStream.Seek(centralDirOffset, SeekOrigin.Begin);
                        this.KmzFileStream.Read(this.CentralDirImage, 0, centralSize);

                        // Leave the pointer at the begining of central dir, to append new files
                        this.KmzFileStream.Seek(centralDirOffset, SeekOrigin.Begin);
                        return true;
                    }
                } while (this.KmzFileStream.Position > 0);
            }
            catch { }

            return false;
        }

        private UpdatableItem extractUpdatableItem(string itemPath)
        {
            List<Kmz.KmzFileEntry> dir = ReadCentralDir();
            foreach (Kmz.KmzFileEntry elt in dir)
            {
                bool found;
                if (itemPath.Contains("/")) found = elt.FilenameInKmz.Equals(itemPath, StringComparison.InvariantCultureIgnoreCase);
                else found = elt.FilenameInKmz.EndsWith(itemPath, StringComparison.InvariantCultureIgnoreCase);

                if (found)
                {
                    UpdatableItem kml = new UpdatableItem(elt, new MemoryStream((int)elt.FileSize));
                    ExtractFile(kml);
                    return kml;
                }
            }
            return null;
        }

        #endregion

        #region IDisposable Members
        /// <summary>
        /// Closes the Kmz file stream
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }
        #endregion
    }
}
