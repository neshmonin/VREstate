﻿using System;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace Vre.Server
{
    public class Utilities
    {
        public static string ExplodeException(Exception ex)
        {
            return ExplodeException(ex, string.Empty);
        }

        private static string ExplodeException(Exception ex, string shift)
        {
            StringBuilder text = new StringBuilder();

            while (ex != null)
            {
                text.AppendFormat("{0}{1}\r\n{2}\r\n", shift, ex.Message, ex.StackTrace);

                ReflectionTypeLoadException rex = ex as ReflectionTypeLoadException;
                if (rex != null)
                {
                    foreach (Exception iex in rex.LoaderExceptions)
                        text.Append(ExplodeException(iex, shift + "===>"));
                }

                shift += "    ";
                ex = ex.InnerException;
            }

            return text.ToString();
        }

        public static string ToCsv<T>(IEnumerable<T> items)
        {
            StringBuilder result = new StringBuilder();
            bool comma = false;
            foreach (T item in items)
            {
                if (comma) result.Append(',');
                else comma = true;
                result.Append(item);
            }
            return result.ToString();
        }

        public static List<string> FromCsv(string csv)
        {
            string[] parts = csv.Split(',');
            return new List<string>(parts);
        }

        //public string ToCsv<T>(T[] items)
        //{
        //    StringBuilder result = new StringBuilder();
        //    int cnt = items.Length;
        //    for (int idx = 0; idx < cnt; idx++)
        //    {
        //        if (idx > 0) result.Append(',');
        //        result.Append(items[idx]);
        //    }
        //    return result.ToString();
        //}

        //http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/3928b8cb-3703-4672-8ccd-33718148d1e3/
        public static string BytesToHexStr(byte[] p)
        {
            // remove comments to get "0x" added as a prefix
            char[] c = new char[p.Length * 2/* + 2*/];
            byte b;

            //c[0] = '0'; c[1] = 'x';

            for (int y = 0, x = 0/*2*/; y < p.Length; ++y, ++x)
            {
                b = ((byte)(p[y] >> 4));
                c[x] = (char)(b > 9 ? b + 0x37 : b + 0x30);
                b = ((byte)(p[y] & 0xF));
                c[++x] = (char)(b > 9 ? b + 0x37 : b + 0x30);
            }

            return new string(c);
        }

        public static byte[] HexStrToBytes(string str, int offset, int step, int tail)
        {
            byte[] b = new byte[(str.Length - offset - tail + step) / (2 + step)];
            byte c1, c2;
            int l = str.Length - tail;
            int s = step + 1;
            for (int y = 0, x = offset; x < l; ++y, x += s)
            {
                c1 = (byte)str[x];
                if (c1 > 0x60) c1 -= 0x57;
                else if (c1 > 0x40) c1 -= 0x37;
                else c1 -= 0x30;
                c2 = (byte)str[++x];
                if (c2 > 0x60) c2 -= 0x57;
                else if (c2 > 0x40) c2 -= 0x37;
                else c2 -= 0x30;
                b[y] = (byte)((c1 << 4) + c2);
            }
            return b;
        }

        public static long DirSize(string path)
        {
            return DirSize(path, true);
        }

        public static long DirSize(string path, bool withSubdirs)
        {
            return DirSize(new DirectoryInfo(path), withSubdirs);
        }

        public static long DirSize(DirectoryInfo d, bool withSubdirs)
        {
            long Size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                Size += fi.Length;
            }
            if (withSubdirs)
            {
                // Add subdirectory sizes.
                DirectoryInfo[] dis = d.GetDirectories();
                foreach (DirectoryInfo di in dis)
                {
                    Size += DirSize(di, true);
                }
            }
            return (Size);
        }

        private static readonly char[] _radixXX = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 
            'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        private static readonly int _radixBase = _radixXX.Length;
        private static readonly long _refBase = new DateTime(2012, 1, 1).Ticks;

        public static string GenerateReferenceNumber()
        {
            return GenerateReferenceNumber(DateTime.UtcNow);
        }

        public static string GenerateReferenceNumber(DateTime time)
        {
            string result = string.Empty;
            long t = (time.Ticks - _refBase) / 10000;

            while (t > 0)
            {
                result = _radixXX[t % _radixBase] + result;
                t /= _radixBase;
            }

            return result;
        }
    }
}