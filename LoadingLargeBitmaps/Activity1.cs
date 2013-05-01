using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace LoadingLargeBitmaps
{
    [Activity(Label = "LoadingLargeBitmaps", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var imageView = this.FindViewById<ImageView>(Resource.Id.imageView);

            var options = new BitmapFactory.Options
                              {
                                  InJustDecodeBounds = true
                              };

            // get the size and mime type of the image
            BitmapFactory.DecodeResource(Resources, Resource.Drawable.Koala, options);
            var imageHeight = options.OutHeight;
            var imageWidth = options.OutWidth;
            var imageType = options.OutMimeType;

            Toast.MakeText(this, string.Format("Size=[{0}, {1}] - Type=[{2}]", imageWidth, imageHeight, imageType), ToastLength.Long).Show();

            // load the large image into the ImageView
            using (var bmp = DecodeSampledBitmapFromResource(Resources, Resource.Drawable.Koala, 100, 100))
            {
                imageView.SetImageBitmap(bmp);
            }

            ThreadPool.QueueUserWorkItem(
                delegate
                    {
                        while (true)
                        {
                            // load the large image into the ImageView
                            using (var bmp = BitmapFactory.DecodeResource(Resources, Resource.Drawable.Koala))
                            {
                                // this is an infinte loop that should never cause the app to crash
                                Console.WriteLine("Processing...");

                                // dispose of the image in both Mono and Java
                                bmp.Recycle();
                            }
                        }
                    });
        }

        public static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            var height = (float) options.OutHeight;
            var width = (float) options.OutWidth;
            var inSampleSize = 1D;

            if (height > reqHeight || width > reqWidth)
            {
                inSampleSize = width > height
                                   ? height/reqHeight
                                   : width/reqWidth;
            }

            return (int) inSampleSize;
        }

        public static Bitmap DecodeSampledBitmapFromResource(Resources res, int resId, int reqWidth, int reqHeight)
        {
            // First decode with inJustDecodeBounds=true to check dimensions
            var options = new BitmapFactory.Options {InJustDecodeBounds = true};
            BitmapFactory.DecodeResource(res, resId, options);

            // Calculate inSampleSize
            options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);

            // Decode bitmap with inSampleSize set
            options.InJustDecodeBounds = false;
            return BitmapFactory.DecodeResource(res, resId, options);
        }
    }
}
