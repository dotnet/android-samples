using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace StandardControls {

    [Activity(Label = "GridView")]
    public class GridViewScreen : Activity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.GridView);

            var gridview = FindViewById<GridView>(Resource.Id.Grid);
            gridview.Adapter = new ImageAdapter(this);

            gridview.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs args) {
                Toast.MakeText(this, args.Position.ToString(), ToastLength.Short).Show();
            };
        }

        public class ImageAdapter : BaseAdapter {
            Context context;

            public ImageAdapter(Context c)
            {
                context = c;
            }

            public override int Count
            {
                get { return thumbIds.Length; }
            }

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
                ImageView imageView;

                if (convertView == null) {  // if it's not recycled, initialize some attributes  
                    imageView = new ImageView(context);
                    imageView.LayoutParameters = new GridView.LayoutParams(120, 80);  // images are all the same dimension
                    imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
                    imageView.SetPadding(8, 8, 8, 8);
                } else {
                    imageView = (ImageView)convertView;
                }

                imageView.SetImageResource(thumbIds[position]);
                return imageView;
            }

            // references to our images  
            int[] thumbIds = {  
				Resource.Drawable.Beach, Resource.Drawable.Plants,  
				Resource.Drawable.Seeds, Resource.Drawable.Shanghai,  
				Resource.Drawable.Beach, Resource.Drawable.Plants,  
				Resource.Drawable.Seeds, Resource.Drawable.Shanghai,  
				Resource.Drawable.Beach, Resource.Drawable.Plants,  
				Resource.Drawable.Seeds, Resource.Drawable.Shanghai, 
				Resource.Drawable.Beach, Resource.Drawable.Plants,  
				Resource.Drawable.Seeds, Resource.Drawable.Shanghai,  
				Resource.Drawable.Beach, Resource.Drawable.Plants,  
				Resource.Drawable.Seeds, Resource.Drawable.Shanghai,   
				Resource.Drawable.Beach, Resource.Drawable.Plants
			};
        }
    }
}

