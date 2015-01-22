using System.Threading.Tasks;

using Android.App;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using Android.Graphics.Drawables;

namespace LoadingLargeBitmaps
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        ImageView _imageView;
        TextView _originalDimensions;
        TextView _resizedDimensions;

        public static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            float height = options.OutHeight;
            float width = options.OutWidth;
            double inSampleSize = 1D;

            if (height > reqHeight || width > reqWidth)
            {
                int halfHeight = (int)(height / 2);
                int halfWidth = (int)(width / 2);

                // Calculate a inSampleSize that is a power of 2 - the decoder will use a value that is a power of two anyway.
                while ((halfHeight / inSampleSize) > reqHeight && (halfWidth / inSampleSize) > reqWidth)
                {
                    inSampleSize *= 2;
                }

			}

            return (int)inSampleSize;
        }

        public async Task<Bitmap> LoadScaledDownBitmapForDisplayAsync(Resources res, BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Calculate inSampleSize
            options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);

            // Decode bitmap with inSampleSize set
			options.InJustDecodeBounds = false;

            return await BitmapFactory.DecodeResourceAsync(res, Resource.Drawable.samoyed, options);
        }

		protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            _originalDimensions = FindViewById<TextView>(Resource.Id.original_image_dimensions_textview);
            _resizedDimensions = FindViewById<TextView>(Resource.Id.resized_image_dimensions_textview);
            _imageView = FindViewById<ImageView>(Resource.Id.resized_imageview);


			BitmapFactory.Options options = await GetBitmapOptionsOfImage();

			Bitmap bitmapToDisplay = await LoadScaledDownBitmapForDisplayAsync (Resources, options, 150, 150);
			_imageView.SetImageBitmap(bitmapToDisplay);

			_resizedDimensions.Text = string.Format("Reduced the image {0}x", options.InSampleSize);
        }


        async Task<BitmapFactory.Options> GetBitmapOptionsOfImage()
        {
            BitmapFactory.Options options = new BitmapFactory.Options
                                            {
                                                InJustDecodeBounds = true
                                            };

			// The result will be null because InJustDecodeBounds == true.
			Bitmap result=  await BitmapFactory.DecodeResourceAsync(Resources, Resource.Drawable.samoyed, options);


            int imageHeight = options.OutHeight;
            int imageWidth = options.OutWidth;

            _originalDimensions.Text = string.Format("Original Size= {0}x{1}", imageWidth, imageHeight);

            return options;
        }
    }
}
