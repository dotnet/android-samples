namespace LoadingLargeBitmaps
{
    using System;
    using System.Threading;
	using System.Threading.Tasks;

    using Android.App;
    using Android.Content.Res;
    using Android.Graphics;
    using Android.OS;
    using Android.Widget;

    [Activity (Label = "LoadingLargeBitmaps", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        public static int CalculateInSampleSize (BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            float height = (float)options.OutHeight;
            float width = (float)options.OutWidth;
            double inSampleSize = 1D;

            if (height > reqHeight || width > reqWidth) {
                inSampleSize = width > height ? height / reqHeight : width / reqWidth;
            }

            return (int)inSampleSize;
        }

        public async Task<Bitmap> DecodeSampledBitmapFromResourceAsync (Resources res, int resId, int reqWidth, int reqHeight)
        {
            // First decode with inJustDecodeBounds=true to check dimensions
            BitmapFactory.Options options = new BitmapFactory.Options {
				InJustDecodeBounds = true,
				InPurgeable = true
			};

			// Suspends the execution of the method until DecodeResourceAsync is complete
			await BitmapFactory.DecodeResourceAsync (res, resId, options).ConfigureAwait (false);

            // Calculate inSampleSize
            options.InSampleSize = CalculateInSampleSize (options, reqWidth, reqHeight);

            // Decode bitmap with inSampleSize set
            options.InJustDecodeBounds = false;

			// Don't configure an awaiter to avoid deadlock due to Result call from synchronous OnCreate
			return await BitmapFactory.DecodeResourceAsync (res, resId, options).ConfigureAwait (false);
		}

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);
            SetContentView (Resource.Layout.Main);
            ImageView imageView = FindViewById <ImageView> (Resource.Id.imageView);

            BitmapFactory.Options options = new BitmapFactory.Options {
				InJustDecodeBounds = true
			};

			// Update the GUI while DecodeResourceAsync executes
			var task = BitmapFactory.DecodeResourceAsync (Resources, Resource.Drawable.Koala, options);
			Toast.MakeText (this, "Decoding Completed: " + task.IsCompleted, ToastLength.Long).Show ();

			var bitmap = task.Result; // Synchronous wait
			Toast.MakeText (this, "Decoding Completed: " + task.IsCompleted, ToastLength.Long).Show ();

			// Get the size and mime type of the image after calling DecodeResource
			int imageHeight = options.OutHeight;
			int imageWidth = options.OutWidth;
			string imageType = options.OutMimeType;

            Toast.MakeText (this, string.Format ("Size=[{0}, {1}] - Type=[{2}]", imageWidth, imageHeight, imageType), ToastLength.Long).Show();

			// Load the large image into the ImageView by waiting for the tasks result (bitmap)
			using (var bmpTask = DecodeSampledBitmapFromResourceAsync (Resources, Resource.Drawable.Koala, 100, 100)) {
				imageView.SetImageBitmap (bmpTask.Result); // Synchronous wait
			}

            ThreadPool.QueueUserWorkItem (async delegate {
				// This is an infinte loop that should never cause the app to crash
				while (true) {
					using (Bitmap bmp = await BitmapFactory.DecodeResourceAsync (Resources, Resource.Drawable.Koala)) {
						Console.WriteLine ("Processing...");

						// Dispose of the image in both Mono and Java
						bmp.Recycle ();
					}
				}
			});
        }
    }
}
