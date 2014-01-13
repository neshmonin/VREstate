using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Vre.Server.RemoteService;

namespace Vre.Server.HttpService
{
	class FormDataItem : IFormDataItem
	{
		public string ContentType { get; private set; }
		public string Name { get; private set; }
		public string FileName { get; private set; }
		public Stream Data { get; private set; }

		public FormDataItem(string contentType, string name, string fileName, Stream data)
		{
			ContentType = contentType;
			Name = name;
			FileName = fileName;
			Data = data;
		}
	}

	class MultipartFormData : IMultipartFormData
	{
		private MemoryStream _ms;
		private List<FormDataItem> _items;

		public ICollection<IFormDataItem> Items { get { return _items.ToArray(); } }

		private MultipartFormData() 
		{
			_ms = new MemoryStream();
			_items = new List<FormDataItem>();
		}

		public static IMultipartFormData Parse(HttpListenerRequest rq)
		{
			MultipartFormData result = new MultipartFormData();

			var boundaryMarker = getBoundaryMarker(rq.ContentEncoding, rq.ContentType);
			
			rq.InputStream.CopyTo(result._ms, 4096);

			var buffer = result._ms.GetBuffer();
			var length = result._ms.Length;
			var pos = buffer.IndexOfPattern(boundaryMarker, 0L, length);
			var init = true;

			if (pos >= 0)
			{
				pos += boundaryMarker.Length;

				// capture line break sequence after first found boundary marker
				var pos1 = pos;
				for (; pos1 < length; pos1++)
				{
					var c = rq.ContentEncoding.GetChars(buffer, (int)pos1, 1)[0];  // TODO: 2Gb buffer limit
					if ((c != '\r') && (c != '\n')) break;
				}

				// append line break to _front_ of boundary marker byte pattern
				var nbm = new byte[pos1 - pos + boundaryMarker.Length];
				var lbl = pos1 - pos;
				Array.Copy(buffer, pos, nbm, 0, lbl);
				Array.Copy(boundaryMarker, 0, nbm, lbl, boundaryMarker.Length);
				boundaryMarker = nbm;
			}

			while (pos >= 0)
			{
				if (init) init = false;
				else pos += boundaryMarker.Length;

				var pos2 = buffer.IndexOfPattern(boundaryMarker, pos, length);
				if (pos2 < 0) break;

				//
				// Read part headers
				//
				string cDisposition = null, cType = null, cEncoding = null;
				readString(buffer, rq.ContentEncoding, ref pos);  // skip line break after boundary marker
				do
				{
					var line = readString(buffer, rq.ContentEncoding, ref pos);
					if (string.IsNullOrEmpty(line)) break;

					var p0 = line.Split(':');
					if (p0.Length != 2) continue;

					var head = p0[0].Trim().ToLower();
					var value = p0[1].Trim().ToLower();
					if (head.Equals("content-disposition")) cDisposition = value;
					else if (head.Equals("content-type")) cType = value;
					else if (head.Equals("content-transfer-encoding")) cEncoding = value;
				} while (true);

				//
				// Process content-disposition
				//
				string cName = null, cFilename = null;
				if (cDisposition != null)
				{
					foreach (var token in cDisposition.Split(';'))
					{
						// var t = token.Trim();  // No need for value-less tokens
						var p2 = token.Split('=');
						if (p2.Length != 2) continue;
						var key = p2[0].Trim();
						if (key.Equals("name")) cName = deQuote(p2[1].Trim());
						else if (key.Equals("filename")) cFilename = deQuote(p2[1].Trim());
					}
				}

				// TODO: cEncoding processing!
				/*
				 "BASE64" / "QUOTED-PRINTABLE" / 
					 "8BIT"   / "7BIT" / 
					 "BINARY" / x-token
				 */
				//if (!cEncoding.Equals("binary"))
				if (cEncoding != null) ServiceInstances.Logger.Debug("MFD: Content-Encoding={0}", cEncoding);

				result._items.Add(new FormDataItem(cType, cName, cFilename, 
					new VirtualMemoryStream(buffer, pos, pos2 - pos)));

				pos = pos2;
			}
			return result;
		}

		private static byte[] getBoundaryMarker(Encoding enc, string contentType)
		{
			foreach (var part in contentType.Split(';'))
			{
				var kv = part.Split('=');
				if (kv.Length != 2) continue;
				if (!kv[0].Trim().Equals("boundary")) continue;
				return enc.GetBytes("--" + kv[1].Trim());
			}

			ServiceInstances.Logger.Error("MFD: CT={0}", contentType);
			throw new ArgumentException("Content-Type does not specify boundary");
		}

		private static string deQuote(string value)
		{
			if (!string.IsNullOrEmpty(value) && value.StartsWith("\""))
				return value.Substring(1, value.Length - 2);
			else
				return value;
		}

		private static string readString(byte[] buffer, Encoding enc, ref long startPos)
		{
			StringBuilder result = new StringBuilder();

			long blen = buffer.Length;
			bool cr = false, lf = false;
			for (; startPos < blen; startPos++)
			{
				// TODO: multibyte encodings!!!
				var c = enc.GetChars(buffer, (int)startPos, 1)[0];  // TODO: 2Gb buffer limit
				if (c == '\r') { if (cr) break; else cr = true; }
				else if (c == '\n') { if (lf) break; else lf = true; }
				else if (cr || lf) break;
				else result.Append(c);
			}

			return result.ToString();
		}
	}
}