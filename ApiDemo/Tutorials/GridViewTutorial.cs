using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Views;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Tutorials/Grid View")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class GridViewTutorial : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.GridViewTutorial);

			var gridview = FindViewById<GridView> (Resource.Id.gridview);
			gridview.Adapter = new ImageAdapter (this);

			gridview.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args) {
				Toast.MakeText (this, args.Position.ToString (), ToastLength.Short).Show ();
			};
		}

		public class ImageAdapter : BaseAdapter
		{
			Context context;

			public ImageAdapter (Context c)
			{
				context = c;
			}

			public override int Count {
				get { return thumbIds.Length; }
			}

			public override Java.Lang.Object GetItem (int position)
			{
				return null;
			}

			public override long GetItemId (int position)
			{
				return 0;
			}

			// create a new ImageView for each item referenced by the Adapter  
			public override View GetView (int position, View convertView, ViewGroup parent)
			{
				ImageView imageView;

				if (convertView == null) {  // if it's not recycled, initialize some attributes  
					imageView = new ImageView (context);
					imageView.LayoutParameters = new GridView.LayoutParams (85, 85);
					imageView.SetScaleType (ImageView.ScaleType.CenterCrop);
					imageView.SetPadding (8, 8, 8, 8);
				} else {
					imageView = (ImageView)convertView;
				}

				imageView.SetImageResource (thumbIds[position]);
				return imageView;
			}

			// references to our images  
			int[] thumbIds = {  
				Resource.Drawable.sample_2, Resource.Drawable.sample_3,  
				Resource.Drawable.sample_4, Resource.Drawable.sample_5,  
				Resource.Drawable.sample_6, Resource.Drawable.sample_7,  
				Resource.Drawable.sample_0, Resource.Drawable.sample_1,  
				Resource.Drawable.sample_2, Resource.Drawable.sample_3,  
				Resource.Drawable.sample_4, Resource.Drawable.sample_5,  
				Resource.Drawable.sample_6, Resource.Drawable.sample_7,  
				Resource.Drawable.sample_0, Resource.Drawable.sample_1,  
				Resource.Drawable.sample_2, Resource.Drawable.sample_3,  
				Resource.Drawable.sample_4, Resource.Drawable.sample_5,  
				Resource.Drawable.sample_6, Resource.Drawable.sample_7  
			};
		}
	}
}