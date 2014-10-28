using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;

namespace Vre.Server.RemoteService
{
    internal class GenerationService
    {
        public const string ServicePathPrefix = ServicePathElement0 + "/";
        private const string ServicePathElement0 = "gen";

        private static string _imgCachePath = null;
        private static HashAlgorithm _hashFunction = MD5.Create();

        private const double AvgFontImageHeightToFontHeightRatio = 1.24;
        private const double AvgLetterWidthToHeightRatio = 0.61;

        enum ServiceType { TextImage }

        public static void ProcessClientRequest(IServiceRequest request)
        {
            string x = request.Request.ConstructClientRootUri();
            ServicePathPrefix.Equals(x);
            // x to be used to generate references to other server's resources (e.g. images, models etc.)

            ServiceType st;

            getPathElements(request.Request.Path, out st);

            switch (st)
            {
                case ServiceType.TextImage:
                    processTextImageRequest(request.Request.Query, request.Response);
                    return;
            }

            throw new NotImplementedException();
        }

        private static void getPathElements(string path, out ServiceType st)
        {
            string[] elements = path.Split('/');
            if ((elements.Length < 2)) throw new ArgumentException("Object path is invalid (0).");

            if (!elements[0].Equals(ServicePathElement0)) throw new ArgumentException("Object path is invalid (1).");

            if (elements[1].Equals("txt")) st = ServiceType.TextImage;
            else throw new ArgumentException("Object path is invalid (2).");

            // ...
        }

        private static void processTextImageRequest(ServiceQuery query, IResponseData resp)
        {
            string text = query.GetParam("text", "Test");
            int height = query.GetParam("height", 12);
            Color textColor = readColorParam(query, "txtclr", Color.Black);
			if (Color.Black == textColor) textColor = readColorParam(query, "txtClr", Color.Black);  // OBSOLETE URI
            Color shadowColor = readColorParam(query, "shdclr", Color.DarkGray);
			if (Color.DarkGray == shadowColor) shadowColor = readColorParam(query, "shdClr", Color.DarkGray);  // OBSOLETE URI
            bool frame = (query.GetParam("frame", 0) != 0);

            string type;
            string location = null;
            generateTextImage(text, height, textColor, shadowColor, frame, 
                out type, resp.DataStream, ref location, true);
            resp.DataPhysicalLocation = location;
            resp.DataStreamContentType = type;
            resp.ResponseCode = HttpStatusCode.OK;
        }

        private static Color readColorParam(ServiceQuery query, string paramName, Color defaultValue)
        {
            Color result = defaultValue;

            string text = query.GetParam(paramName, null);
            if (text != null)
            {
                int value;
                if (text.StartsWith("x"))
                {
                    if (int.TryParse(text.Substring(1), 
                        NumberStyles.AllowHexSpecifier, NumberFormatInfo.InvariantInfo, out value))
                        result = Color.FromArgb((int)((uint)value | 0xFF000000L));
                }
                else if (text.StartsWith("0x"))
                {
                    if (int.TryParse(text.Substring(2), 
                        NumberStyles.AllowHexSpecifier, NumberFormatInfo.InvariantInfo, out value))
                        result = Color.FromArgb((int)((uint)value | 0xFF000000L));
                }
                else
                {
                    if (int.TryParse(text, out value))
                        result = Color.FromArgb((int)((uint)value | 0xFF000000L));
                }
            }

            return result;
        }

        private static void generateTextImage(string text,
                                              int height,
                                              Color textColor, Color shadowColor, bool frame, 
                                              out string type, System.IO.Stream resp, ref string resourceLocation,
                                              bool searchCache)
        {
            // initialize cache path if necessary
            //
            if (null == _imgCachePath)
            {
                string path = Path.Combine(Configuration.GeneratedImageCache.RootPath.Value, "images");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                _imgCachePath = path;  // don't do this before so that exception being thrown shall leave this as null
            }

			var minCachedImagePx = Configuration.GeneratedImageCache.MinCachedImagekPx.Value * 1000;
			var maxGeneratedImagePx = Configuration.GeneratedImageCache.MaxGeneratedImagekPx.Value * 1000;
			
			int imgpx = calcAvgImagePx(height, text.Length);
            if (imgpx > maxGeneratedImagePx) throw new ArgumentOutOfRangeException("Image requested too large.");
            if (searchCache)
            {
                if (imgpx < minCachedImagePx) searchCache = false;
            }

            if (searchCache)
            {
                // compute file name
                //
                string hash = string.Format("{0}{1}{2}{3}{4}", text, height, textColor, shadowColor, frame);
                byte[] hashCode = _hashFunction.ComputeHash(Encoding.Unicode.GetBytes(hash));
                string filePath = Path.Combine(_imgCachePath, Utilities.BytesToHexStr(hashCode));

                if (!File.Exists(filePath))
                {
                    // generate file and copy stream to output
                    //
                    using (FileStream fs = File.Create(filePath))
                    {
                        generateTextImage(text, ref height, ref textColor, ref shadowColor, frame, fs);
                        ServiceInstances.FileCache.AddManagedFile(filePath);
                    }
                    resourceLocation = filePath;
                }
                else
                {
                    resourceLocation = filePath;
                }
            }
            else
            {
                // generate image and stream it out
                //
                generateTextImage(text, ref height, ref textColor, ref shadowColor, frame, resp);
            }

            type = "png";
        }

        private static int calcAvgImagePx(int fontHeight, int letterCount)
        {
            int h = (int)(AvgFontImageHeightToFontHeightRatio * (double)fontHeight);
            int w = (int)(AvgLetterWidthToHeightRatio * (double)(fontHeight * letterCount));
            return h * w;
        }

		private static readonly Point _dummyPoint = new Point(0, 0);
		private static readonly StringFormat _stringFormatWithSpaces = new StringFormat(StringFormatFlags.MeasureTrailingSpaces);
		
		private static void generateTextImage(string text, ref int height, 
            ref Color textColor, ref Color shadowColor, bool frame, Stream output)
        {
            using (Font oFont = new Font("Arial", height, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                int width;
                using (Bitmap bm = new Bitmap(4, height))
                {
                    using (Graphics gr = Graphics.FromImage((Image)bm))
                    {
                        //gr.SmoothingMode = Graphics.SmoothingMode.AntiAlias;
                        SizeF textSizeF = gr.MeasureString(text, oFont, _dummyPoint, _stringFormatWithSpaces);
                        width = (int)textSizeF.Width;
                        height = (int)textSizeF.Height;
                    }
                }

                using (Bitmap bm = new Bitmap(width, height))
                {
                    using (Graphics gr = Graphics.FromImage((Image)bm))
                    {
                        using (SolidBrush txtBrush = new SolidBrush(textColor))
                        {
                            using (SolidBrush ShdBrush = new SolidBrush(shadowColor))
                            {
                                Rectangle rect = new Rectangle(0, 0, bm.Width, bm.Height);
                                if (!frame)
                                {
                                    gr.FillRectangle(Brushes.Transparent, rect);
                                }
                                else
                                {
                                    rect = new Rectangle(0, 0, bm.Width, bm.Height);
                                    Color bgBrushClr = Color.FromArgb(70, textColor);
                                    using (SolidBrush bgBrush = new SolidBrush(bgBrushClr))
                                    {
                                        gr.FillRectangle(bgBrush, rect);
                                        //gr.FillEllipse(bgBrush, rect);
                                    }

                                    //gr.DrawRectangle(new Pen(ShdBrush, 2), rect);
                                    rect = new Rectangle(2, 2, bm.Width - 4, bm.Height - 4);
                                    gr.DrawRectangle(new Pen(txtBrush, 4), rect);
                                    //gr.DrawEllipse(new Pen(txtBrush, 3), rect);
                                }

                                gr.DrawString(text, oFont, ShdBrush, -1, -2);

                                gr.DrawString(text, oFont, txtBrush, 0, -1);
                            }
                        }

                        bm.Save(output, ImageFormat.Png);
                    }
                }
            }
        }
    }
}