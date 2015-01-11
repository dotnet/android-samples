using System;
 using System.Collections.Generic;
 using System.Linq;
 using System.Text;
 using Android.App;
 using Android.Content;
 using Android.OS;
 using Android.Runtime;
 using Android.Views;
 using Android.Widget;
 using Android.Graphics;
 using Android;
 using Android.Util;
 using Android.Telephony;
 using System.Threading.Tasks;
 
 namespace KitKat
 {
	/// <summary>
	/// This class will take care of scaling an image down so that it's an appropriate size
	/// for an ImageView. This is to help reduce memory usage.
	/// </summary>
	class ImageViewHelper
	{
		Context context;
		 
		public ImageViewHelper(Context context)
		{
			this.context = context;
		}

		/// <summary>
		/// This method will take the image at a given URI, resize it to conserve memory, 
		/// and then display it in the provided ImageView.
		/// </summary>
		/// <returns>The picture async.</returns>
		/// <param name="imageView">Image view.</param>
		/// <param name="uri">URI.</param>
		public async Task DisplayPictureAsync(ImageView imageView, Android.Net.Uri uri)
		{
			Bitmap bitmapToDisplay;
			 
			var thumbnailSize = GetThumbnailSize (imageView);
			 
			var bitmapOptions = new BitmapFactory.Options ();
			bitmapOptions.InSampleSize = await GetRatioToScaleBitmapAsync (uri, thumbnailSize);
			bitmapOptions.InDither = true;
			bitmapOptions.InPreferredConfig = Bitmap.Config.Argb8888;
			 
			using (var inputStream = context.ContentResolver.OpenInputStream (uri)) {
				bitmapToDisplay = await BitmapFactory.DecodeStreamAsync (inputStream, null, bitmapOptions);
			}
			 
			imageView.SetImageBitmap (bitmapToDisplay);
		}
		 
		/// <summary>
		/// Calculates a scaling ration based on a given thumbnail size.
		/// </summary>
		/// <returns>The ratio to scale bitmap.</returns>
		/// <param name="uri">URI.</param>
		/// <param name="thumbnailSize">Thumbnail size.</param>
		async Task<int> GetRatioToScaleBitmapAsync(Android.Net.Uri uri, int thumbnailSize)
		{
			BitmapFactory.Options onlyBoundsOption;
			using (var inputStream = context.ContentResolver.OpenInputStream (uri)) {
				onlyBoundsOption = new BitmapFactory.Options ();
				onlyBoundsOption.InJustDecodeBounds = true;
				onlyBoundsOption.InPreferredConfig = Bitmap.Config.Argb8888;
				await BitmapFactory.DecodeStreamAsync (inputStream, null, onlyBoundsOption);
			}
			 
			if ((onlyBoundsOption.OutHeight == -1) || (onlyBoundsOption.OutWidth == -1)) {
				return 1;
			}
			 
			 
			var originalSize = (onlyBoundsOption.OutHeight > onlyBoundsOption.OutWidth) ? onlyBoundsOption.OutHeight : onlyBoundsOption.OutWidth;
			var ratio = (originalSize > thumbnailSize) ? (originalSize / thumbnailSize) : 1;
			 
			// We want to scale according to a power of two, Android is faster that way.
			return GetPowerOfTwo (ratio);
			 
		}
		 
		/// <summary>
		/// Thumbnail size is the lessor of height or width.
		/// </summary>
		/// <returns>The thumbnail size.</returns>
		/// <param name="imageView">Image view.</param>
		int GetThumbnailSize(ImageView imageView)
		{
			int size;
			if (imageView.Width > imageView.Height) {
				size = imageView.Height;
			} else {
				size = imageView.Width;
			}
			return size;
		}
		 
		/// <summary>
		/// This method will determine the next biggest power of two for a given integer.
		/// </summary>
		/// <returns>The power of two.</returns>
		/// <param name="x">An integer value.</param>
		int GetPowerOfTwo(int x)
		{
			x |= (x >> 1);
			x |= (x >> 2);
			x |= (x >> 4);
			x |= (x >> 8);
			x |= (x >> 16);
			return (x + 1);
		}
	}
}