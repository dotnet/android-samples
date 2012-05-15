using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace StandardControls {

    [Activity(Label = "Gallery")]
    public class GalleryScreen : Activity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Gallery);

            Gallery gallery = (Gallery)FindViewById<Gallery>(Resource.Id.Gallery);

            gallery.Adapter = new ImageAdapter(this);

            gallery.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs args) {
                Toast.MakeText(this, args.Position.ToString(), ToastLength.Short).Show();
            };
        }

        public class ImageAdapter : BaseAdapter {
            Context context;

            public ImageAdapter(Context c)
            {
                context = c;
            }

            public override int Count { get { return thumbIds.Length; } }

            public override Java.Lang.Object GetItem(int position)
            {
                return null;
            }

            public override long GetItemId(int position)
            {
                return 0;
            }

            // create a new ImageView for each item referenced by the Adapter  
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                ImageView i = new ImageView(context);

                i.SetImageResource(thumbIds[position]);
                i.LayoutParameters = new Gallery.LayoutParams(240, 160);
                i.SetScaleType(ImageView.ScaleType.FitXy);

                return i;
            }

            // references to our images  
            int[] thumbIds = {  
				  Resource.Drawable.Beach
                , Resource.Drawable.Plants
                , Resource.Drawable.Seeds
                , Resource.Drawable.Shanghai
			 };
        }  
    }
}

