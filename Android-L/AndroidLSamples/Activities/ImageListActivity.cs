
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
using AndroidLSamples.Utils;

namespace AndroidLSamples
{
	[Activity (Label = "Select an Image", ParentActivity=typeof(HomeActivity))]			
	public class ImageListActivity : Activity
	{
		GridView grid;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_image_list);
			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetDisplayShowHomeEnabled (true);
			ActionBar.SetIcon (Android.Resource.Color.Transparent);

			grid = FindViewById<GridView> (Resource.Id.grid);

			grid.Adapter = new GridAdapter (this);
			grid.ItemClick += (sender, e) => {
				var intent = new Intent(this, typeof(ImageDetailPaletteActivity));
				intent.PutExtra("id", Photos.Items[e.Position].Id);
				StartActivity(intent);
			};

			// Create your application here
		}
	}

	/// <summary>
	/// Simple grid view adapter. We would optimize this more, but it is just a sample.
	/// </summary>
	public  class GridAdapter : BaseAdapter
	{
		Activity context;
		public GridAdapter(Activity activity)
		{
			context = activity;
		}


		#region implemented abstract members of BaseAdapter
		public override Java.Lang.Object GetItem (int position)
		{
			return position;
		}
		public override long GetItemId (int position)
		{
			return position;
		}
		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			if (convertView == null)
				convertView = context.LayoutInflater.Inflate (Resource.Layout.palette_item, null);

			var item = Photos.Items[position];
			convertView.FindViewById<ImageView> (Resource.Id.imageview_item).SetImageResource (item.Image);
			convertView.FindViewById<TextView> (Resource.Id.textview_name).Text = item.Name;

			return convertView;
		}
		public override int Count {
			get {
				return Photos.Items.Count;
			}
		}
		#endregion
		
	}
}

