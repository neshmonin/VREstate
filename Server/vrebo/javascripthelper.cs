using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using Vre.Server.BusinessLogic;
using System.Text;

namespace Vre.Server
{
    public class JavaScriptHelper
    {
        /// <summary>
        /// Convert business object to JSON.
        /// </summary>
        public static string BoToJson(IClientDataProvider bo)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(bo.GetClientData());
            //return ClientDataToJson(bo.GetClientData());
        }

        /// <summary>
        /// Convert business object to JSON.
        /// </summary>
        public static string ClientDataToJson(ClientData cd)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(cd);
            //return ClientDataToJson(cd);
        }

        /// <summary>
        /// Convert JSON to ClientData object (to be processed by business object).
        /// </summary>
        public static ClientData JsonToClientData(string json)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return new ClientData(jss.DeserializeObject(json) as Dictionary<string, object>);            
        }

        /// <summary>
        /// Convert JSON as UTF8 stream to ClientData object (to be processed by business object).
        /// </summary>
        public static ClientData JsonToClientData(Stream stream)
        {
            StringBuilder text = new StringBuilder();

            do
            {
                byte[] buffer = new byte[1024];
                int cnt = stream.Read(buffer, 0, buffer.Length);
                if (0 == cnt) break;
                text.Append(Encoding.UTF8.GetString(buffer, 0, cnt));
            } while (true);

            if (text.Length > 0) return JsonToClientData(text.ToString());
            else return null;
        }

        #region unused code (dropped in favour of System.Web.Script.Serialization.JavaScriptSerializer)
        //public static readonly DateTime DateTimeMinValue = new DateTime(1970, 1, 1);

        ///// <summary>
        ///// Convert ClientData object to JSON. ClientData values may be any 
        ///// of standard data types, other ClientData or array/list of ClientData.
        ///// </summary>
        ///// <exception cref="ArgumentException">Is thrown if value type is not known.</exception>
        //public static string ClientDataToJson(ClientData data)
        //{
        //    //http://blog.activa.be/index.php/2007/08/writing-a-full-json-serializer-in-100-lines-of-c-code/
        //    bool next = false;
        //    StringBuilder text = new StringBuilder();
        //    text.Append("{");

        //    foreach (KeyValuePair<string, object> kvp in data)
        //    {
        //        object o = kvp.Value;
        //        string ot = null;

        //        if (o == null)
        //        {
        //            ot = "null";
        //        }
        //        else if (o is string)
        //        {
        //            ot = EscapeString(o as string, true);
        //        }
        //        else if (o is sbyte || o is byte || o is short || o is ushort || o is int || o is uint || o is long || o is ulong || o is decimal || o is double || o is float)
        //        {
        //            ot = Convert.ToString(o, NumberFormatInfo.InvariantInfo);
        //        }
        //        else if (o is bool)
        //        {
        //            ot = o.ToString().ToLower();
        //        }
        //        else if (o is char || o is Enum || o is Guid)
        //        {
        //            ot = EscapeString(o.ToString(), true);
        //        }
        //        else if (o is DateTime)
        //        {
        //            ot = "new Date(" + ((DateTime)o - DateTimeMinValue).TotalMilliseconds.ToString("0") + ")";
        //        }
        //        else if (o is ClientData)
        //        {
        //            ot = ClientDataToJson(o as ClientData);
        //        }
        //        else if (o is IEnumerable<ClientData>)
        //        {
        //            bool next2 = false;
        //            StringBuilder text2 = new StringBuilder();
        //            text2.Append("[");

        //            foreach (ClientData cd in o as IEnumerable<ClientData>)
        //            {
        //                text2.AppendFormat("{0}{1}", (next2 ? "," : string.Empty), ClientDataToJson(cd));
        //                next2 = true;
        //            }

        //            text2.Append("]");
        //            ot = text2.ToString();
        //        }
        //        else
        //        {
        //            //ot = EscapeString(o.ToString());
        //            throw new ArgumentException("Unknown value type: " + o.GetType());
        //        }

        //        text.AppendFormat("{0}{1}:{2}", (next ? "," : string.Empty), kvp.Key, ot);
        //        next = true;
        //    }

        //    text.Append("}");
        //    return text.ToString();
        //}

        ///// <summary>
        ///// Escape special string characters to conform to JavaScript standarts.
        ///// </summary>
        //public static string EscapeString(string text, bool addQuotes)
        //{
        //    //http://blog.activa.be/index.php/2007/08/writing-a-full-json-serializer-in-100-lines-of-c-code/
        //    StringBuilder result = new StringBuilder(text.Length * 2);  // best guessed size
        //    if (addQuotes) result.Append('\'');

        //    foreach (char c in text)
        //    {
        //        switch (c)
        //        {
        //            case '"':
        //            case '\\': result.Append("\\" + c); break;
        //            case '\t': result.Append("\\t"); break;
        //            case '\r': result.Append("\\r"); break;
        //            case '\n': result.Append("\\n"); break;
        //            default:
        //            {
        //                if ((c >= ' ') && (c < 128)) result.Append(c);
        //                else result.Append("\\u" + ((int) c).ToString("X4"));
        //            }
        //            break;
        //        }
        //    }

        //    if (addQuotes) result.Append('\'');
        //    return result.ToString();
        //}
        #endregion
    }
}