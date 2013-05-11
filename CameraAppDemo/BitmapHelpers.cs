namespace CameraAppDemo
{
    using System.IO;

    using Android.Graphics;
    using Android.Graphics.Drawables;
    using Android.Widget;

    public static class BitmapHelpers
    {
        /// <summary>
        /// This method will recyle the memory help by a bitmap in an ImageView
        /// </summary>
        /// <param name="imageView">Image view.</param>
        public static void RecycleBitmap(this ImageView imageView)
        {
            if (imageView == null) {
                return;
            }

            Drawable toRecycle = imageView.Drawable;
            if (toRecycle != null) {
                ((BitmapDrawable)toRecycle).Bitmap.Recycle ();
            }
        }


        /// <summary>
        /// Load the image from the device, and resize it to the specified dimensions.
        /// </summary>
        /// <returns>The and resize bitmap.</returns>
        /// <param name="fileName">File name.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
        {
            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options
                                                {
                                                    InPurgeable = true,
                                                    InJustDecodeBounds = true
                                                };
            BitmapFactory.DecodeFile(fileName, options);

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight
                                   ? outHeight / height
                                   : outWidth / width;
            }

            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

            return resizedBitmap;
        }
    }
}
