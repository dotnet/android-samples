using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Views;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Tutorials/Gallery")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class GalleryTutorial : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource  
			SetContentView (Resource.Layout.GalleryTutorial);

			Gallery gallery = (Gallery)FindViewById<Gallery> (Resource.Id.gallery);

			gallery.Adapter = new ImageAdapter (this);

			gallery.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args) {
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

			public override int Count { get { return thumbIds.Length; } }

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
				ImageView i = new ImageView (context);

				i.SetImageResource (thumbIds[position]);
				i.LayoutParameters = new Gallery.LayoutParams (150, 100);
				i.SetScaleType (ImageView.ScaleType.FitXy);

				return i;
			}

			// references to our images  
			int[] thumbIds = {  
				Resource.Drawable.sample_1,  
				Resource.Drawable.sample_2,  
				Resource.Drawable.sample_3,  
				Resource.Drawable.sample_4,  
				Resource.Drawable.sample_5,  
				Resource.Drawable.sample_6,  
				Resource.Drawable.sample_7  
			 };
		}  
	}
}