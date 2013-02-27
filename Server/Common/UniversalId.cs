using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Vre.Server
{
    public class UniversalId
    {
        public enum IdType { Unknown, ReverseRequest, ViewOrder }

        private static Dictionary<char, IdType> _typeCref;
        private static Dictionary<IdType, char> _revTypeCref;

        static UniversalId()
        {
            _typeCref = new Dictionary<char, IdType>();

            _typeCref.Add('R', IdType.ReverseRequest);
            _typeCref.Add('V', IdType.ViewOrder);

            _revTypeCref = new Dictionary<IdType, char>();
            foreach (char t in _typeCref.Keys) _revTypeCref.Add(_typeCref[t], t);
        }

        public static string GenerateUrlId(IdType type, Guid id)
        {
            return _revTypeCref[type] + base64urlencode(id.ToByteArray());
        }

        public static string GenerateUrlId(IdType type, byte[] id)
        {
            return _revTypeCref[type] + base64urlencode(id);
        }

        public static string GenerateUrlId(IdType type, string id)
        {
            return _revTypeCref[type] + base64urlencode(Encoding.UTF8.GetBytes(id));
        }

        public static IdType TypeInUrlId(string urlId)
        {
            IdType result = IdType.Unknown;
            if ((urlId != null) && (urlId.Length > 1))
            {
                char t = urlId[0];
                if (!_typeCref.TryGetValue(t, out result)) result = IdType.Unknown;
            }
            return result;
        }

        public static Guid ExtractAsGuid(string urlId)
        {
            Guid result;
            char t = urlId[0];
            if (_typeCref.ContainsKey(t))
            {
                byte[] bytes = base64urldecode(urlId.Substring(1));
                if (bytes.Length != 16) throw new ArgumentException();
                result = new Guid(bytes);
            }
            else  // legacy
            {
                if (!Guid.TryParseExact(urlId, "N", out result))
                    throw new ArgumentException();
            }
            return result;
        }

        public static byte[] ExtractAsBytes(string urlId)
        {
            byte[] result;
            char t = urlId[0];
            if (_typeCref.ContainsKey(t))
                result = base64urldecode(urlId.Substring(1));
            else
                throw new ArgumentException();
            return result;
        }

        public static string ExtractAsString(string urlId)
        {
            string result;
            char t = urlId[0];
            if (_typeCref.ContainsKey(t))
                result = Encoding.UTF8.GetString(base64urldecode(urlId.Substring(1)));
            else
                throw new ArgumentException();
            return result;
        }

        public static string base64urlencode(byte[] arg)
        {
            string s = Convert.ToBase64String(arg); // Regular base64 encoder
            string[] ss = s.Split('=');
            return ss[0].Replace('+', '-').Replace('/', '_') + (char)(ss.Length - 1 + '0');
        }

        public static byte[] base64urldecode(string arg)
        {
            string s = arg.Replace('-', '+').Replace('_', '/');
            switch (s[s.Length - 1])
            {
                case '0': s = s.Substring(0, s.Length - 1); break;
                case '1': s = s.Substring(0, s.Length - 1) + "="; break;
                case '2': s = s.Substring(0, s.Length - 1) + "=="; break;
                default: throw new System.Exception("Illegal base64url string!");
            }
            return Convert.FromBase64String(s);
        }
    }
}