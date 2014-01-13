using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.IO;
using System.Drawing.Drawing2D;

namespace Vre.Server.Util
{
	// http://stackoverflow.com/questions/249587/high-quality-image-scaling-c-sharp
	class ImageProcessing
	{
		private static Dictionary<string, ImageCodecInfo> encoders = null;

		public static Dictionary<string, ImageCodecInfo> Encoders
		{
			get
			{
				if (encoders == null)
				{
					encoders = new Dictionary<string, ImageCodecInfo>();

					foreach (var codec in ImageCodecInfo.GetImageEncoders())
						encoders.Add(codec.MimeType.ToLower(), codec);
				}

				return encoders;
			}
		}

		[Obsolete("This distorts image proportion; do not use", true)]
		public static Bitmap ResizeImage(Image image, int width, int height)
		{
			Bitmap result = new Bitmap(width, height);

			//set the resolutions the same to avoid cropping due to resolution differences
			result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (Graphics graphics = Graphics.FromImage(result))
			{
				graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
				graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				graphics.DrawImage(image, 0, 0, result.Width, result.Height);
			}

			return result;
		}

		public static void SaveJpeg(string path, Image image, int quality)
		{
			if ((quality < 0) || (quality > 100))
				throw new ArgumentOutOfRangeException("Quality must be in range 0... 100");

			var encoderParams = new EncoderParameters(1);
			encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);

			image.Save(path, Encoders["image/jpeg"], encoderParams);
		}

		public static void SaveJpeg(Stream output, Image image, int quality)
		{
			if ((quality < 0) || (quality > 100))
				throw new ArgumentOutOfRangeException("Quality must be in range 0... 100");

			using (var encoderParams = new EncoderParameters(1))
			{
				encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);

				image.Save(output, Encoders["image/jpeg"], encoderParams);
			}
		}

		/// <summary>
		/// Crops image and does proportional resize of remainings to conform 
		/// to required size.
		/// <para>All arguments are in pixels.</para>
		/// </summary>
		public static Image ConditionImage(Image image,
			int leftCut, int topCut, int rightCut, int bottomCut,
			int maxWidth, int maxHeight)
		{
			Rectangle crop = new Rectangle();
			crop.X = leftCut;
			crop.Y = topCut;
			crop.Width = image.Width - leftCut - rightCut;
			crop.Height = image.Height - topCut - bottomCut;

			if ((crop.Width <= 0) || (crop.Height <= 0))
				throw new ArgumentException("Crop parameters are invalid");

			Bitmap result;

			if ((crop.Width > maxWidth) || (crop.Height > maxHeight))
			{
				double iar = (double)crop.Width / (double)crop.Height;
				double car = (double)maxWidth / (double)maxHeight;
				int w, h;
				if (iar > car)
				{
					w = maxWidth;
					h = (int)((double)maxWidth / iar);
				}
				else
				{
					h = maxHeight;
					w = (int)((double)maxHeight * iar);
				}

				result = new Bitmap(w, h);
			}
			else
			{
				// TODO: This shall result is image stretch-up to the size.
				// Alter if not the behaviour required.
				result = new Bitmap(crop.Width, crop.Height);
			}

			//set the resolutions the same to avoid cropping due to resolution differences
			result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			//var ia = new ImageAttributes();
			//ia.SetWrapMode(WrapMode.TileFlipXY);

			using (var g = Graphics.FromImage(result))
			{
				g.CompositingQuality = CompositingQuality.HighQuality;
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.SmoothingMode = SmoothingMode.HighQuality;

				// http://stackoverflow.com/questions/8992619/how-to-crop-and-resize-image-in-one-step-in-net
				// http://nathanaeljones.com/163/20-image-resizing-pitfalls/
				g.PixelOffsetMode = PixelOffsetMode.HighQuality;
				g.CompositingMode = CompositingMode.SourceOver;

				g.Clear(Color.White);

				g.DrawImage(image, new Rectangle(0, 0, result.Width, result.Height), crop, GraphicsUnit.Pixel);
			}

			return result;
		}
	}
}