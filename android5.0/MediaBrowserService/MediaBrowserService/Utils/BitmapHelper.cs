using System;
using Android.Graphics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MediaBrowserService
{
	public static class BitmapHelper
	{
		static readonly string Tag = LogHelper.MakeLogTag (typeof(BitmapHelper));
		const int MaxReadLimitPerImg = 1024 * 1024;

		public static Bitmap Scale (Bitmap src, int maxWidth, int maxHeight)
		{
			var scaleFactor = Math.Min (((double)maxWidth) / src.Width, ((double)maxHeight) / src.Height);
			return Bitmap.CreateScaledBitmap (src, (int)(src.Width * scaleFactor), (int)(src.Height * scaleFactor), false);
		}

		public static Bitmap Scale (int scaleFactor, Stream iStream)
		{
			var bmOptions = new BitmapFactory.Options ();
			bmOptions.InJustDecodeBounds = false;
			bmOptions.InSampleSize = scaleFactor;
			return BitmapFactory.DecodeStream (iStream, null, bmOptions);
		}

		public static int FindScaleFactor (int targetWidth, int targetHeight, Stream iStream)
		{
			var bmOptions = new BitmapFactory.Options ();
			bmOptions.InJustDecodeBounds = true;
			BitmapFactory.DecodeStream (iStream, null, bmOptions);
			int actualWidth = bmOptions.OutWidth;
			int actualHeight = bmOptions.OutHeight;
			return Math.Min (actualWidth / targetWidth, actualHeight / targetHeight);
		}

		public async static Task<Bitmap> FetchAndRescaleBitmap(string uri, int width, int height)
		{
			try {
				using (var client = new HttpClient ()) {
					Stream stream = await client.GetStreamAsync (uri);
					int scaleFactor = FindScaleFactor (width, height, stream);
					LogHelper.Debug (Tag, String.Format ("Scaling bitmap {0} by factor {1} to support {3}x{4} requested dimension.",
						uri, scaleFactor, width, height));
					return Scale(scaleFactor, stream);
				}
			} catch (IOException e) {
				throw e;
			}
		}
	}
}

