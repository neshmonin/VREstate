using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;

namespace Vre.Server.Model.Kmz
{
    public class Kmz
    {
        private KmzReader _reader;
        private XmlDocument _kml;
        private string _kmzPath;

        public string Name { get; private set; }
        public string Description { get; private set; }
        public Model Model { get; private set; }

        public Kmz(string path, StringBuilder readWarnings)
        {
            _kmzPath = path;
            _reader = KmzReader.Open(_kmzPath);

            Stream s;

            s = _reader.GetFile("doc.kml");
            if (null == s)
                throw new InvalidDataException(string.Format("MDSC09: The model file {0} does not contain root KML entry", _kmzPath));

            _kml = new XmlDocument();
            _kml.Load(s);

            Name = null;
            Description = null;
            Model = null;
            preReadKml(readWarnings);
        }

        private void preReadKml(StringBuilder readWarnings)
        {
            const string RootNodeName = "kml";

            XmlNode node = _kml.FirstChild;

            while (node != null)
            {
                if (node.Name.Equals(RootNodeName))
                {
                    if (Name != null)
                    {
                        if (readWarnings != null) 
                            readWarnings.Append("\r\nMDSC00: Multiple 'kml' nodes in KML are not supported; subsequent nodes ignored.");
                    }
                    else
                    {
                        readKmlNode(node, readWarnings);
                    }
                }
                node = node.NextSibling;
            }
        }

        private void readKmlNode(XmlNode root, StringBuilder readWarnings)
        {
            const string PlacemarkNodeName = "Placemark";
            bool placemarkFound = false;

            XmlNode node = root.FirstChild;

            while (node != null)
            {
                if (node.Name.Equals(PlacemarkNodeName))
                {
                    placemarkFound = true;
                    if (Name != null)
                    {
                        if (readWarnings != null)
                            readWarnings.Append("\r\nMDSC01: Multiple placemarks in KML are not supported; subsequent placemark ignored.");
                    }
                    else
                    {
                        readPlacemarkNode(node, readWarnings);
                    }
                }
                node = node.NextSibling;
            }

            if (!placemarkFound)
                throw new InvalidDataException(string.Format("MDSC10: There is no Placemark section in {0}.", _kmzPath));
        }

        private void readPlacemarkNode(XmlNode root, StringBuilder readWarnings)
        {
            const string NameNodeName = "name";
            const string DescriptionNodeName = "description";
            const string ModelNodeName = "Model";

            XmlNode node = root.FirstChild;
            XmlNode modelNode = null;

            while (node != null)
            {
                if (node.Name.Equals(NameNodeName))
                {
                    if (Name != null)
                    {
                        if (readWarnings != null) readWarnings.Append("\r\nMDSC02: Multiple 'name' nodes in placemark; subsequent node ignored.");
                    }
                    else
                    {
                        Name = node.InnerText;
                    }
                }
                else if (node.Name.Equals(DescriptionNodeName))
                {
                    if (Description != null)
                    {
                        if (readWarnings != null) readWarnings.Append("\r\nMDSC03: Multiple 'description' nodes in placemark; subsequent node ignored.");
                    }
                    else
                    {
                        Description = node.InnerText;
                    }
                }
                else if (node.Name.Equals(ModelNodeName))
                {
                    if (modelNode != null)
                    {
                        if (readWarnings != null) readWarnings.Append("\r\nMDSC04: Multiple models in placemark are not supported; subsequent model ignored.");
                    }
                    else
                    {
                        modelNode = node;
                    }
                }
                node = node.NextSibling;
            }

            if (modelNode != null) Model = new Model(this, modelNode, readWarnings);
        }

        public Stream GetFile(string pathInStore)
        {
            return _reader.GetFile(pathInStore);
        }

        class KmzReader
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

                /// <summary>Overriden method</summary>
                /// <returns>Filename in Kmz</returns>
                public override string ToString()
                {
                    return this.FilenameInKmz;
                }
            }

            public class UpdatableItem
            {
                public KmzFileEntry fileEntry;
                public Stream stream;

                public UpdatableItem(KmzFileEntry fileEntry, Stream stream)
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

            public const string m_versionInfoFileName = "versionInfo";
            public string SiteName { get; set; }
            public static string DeveloperName { get; set; }
            #endregion

            #region Private fields
            // List of files to store
            private List<KmzFileEntry> Files = new List<KmzFileEntry>();
            // Filename of storage file
            private string FileName;
            // Stream object of storage file
            private Stream KmzFileStream;
            // Central dir image
            private byte[] CentralDirImage = null;
            // Existing files in kmz
            private ushort ExistingFiles = 0;
            // Default filename encoder
            private static readonly Encoding DefaultEncoding = Encoding.GetEncoding(437);

            private UpdatableItem m_kml = null;
            private UpdatableItem m_collada = null;

            private static string m_backupKmzName = string.Empty;

            #endregion

            #region Public methods
            /// <summary>
            /// Method to open an existing storage file
            /// </summary>
            /// <param name="_filename">Full path of Kmz file to open</param>
            /// <param name="_access">File access mode as used in FileStream constructor</param>
            /// <returns>A valid Kmz object</returns>
            public static KmzReader Open(string fileName)
            {
                Stream stream = (Stream)new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

                KmzReader kmz = Open(stream);
                kmz.FileName = fileName;
                return kmz;
            }

            public Stream GetFile(string pathInKmz)
            {
                UpdatableItem item = extractUpdatableItem(pathInKmz);
                if (null == item) return null;
                item.stream.Seek(0, SeekOrigin.Begin);
                return item.stream;
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

                m_collada.stream.Seek(0, SeekOrigin.Begin);
                XmlDocument doc = new XmlDocument();
                doc.Load(m_collada.stream);

                return doc;
            }

            public static KmzReader Open(Stream _stream)
            {
                Stream inpStream = _stream;

                if (!_stream.CanSeek)
                {
                    // make a local copy of the file and then use MemoryStream to read it:
                }

                KmzReader kmz = new KmzReader();
                //kmz.FileName = _filename;
                kmz.KmzFileStream = inpStream;

                if (kmz.ReadFileInfo())
                    return kmz;

                throw new System.IO.InvalidDataException();
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
                List<KmzFileEntry> dir = ReadCentralDir();
                foreach (KmzFileEntry elt in dir)
                {
                    bool found = elt.FilenameInKmz.Equals(itemPath, StringComparison.InvariantCultureIgnoreCase);

                    //if (itemPath.Contains("/")) found = elt.FilenameInKmz.Equals(itemPath, StringComparison.InvariantCultureIgnoreCase);
                    //else found = elt.FilenameInKmz.EndsWith(itemPath, StringComparison.InvariantCultureIgnoreCase);

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
        }
    }
}