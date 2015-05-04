using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Supportv7Pallete.Utils;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Supportv7Pallete
{
	[Activity (Label = "Pallete Sample", MainLauncher = true)]			
	public class ImageListActivity : AppCompatActivity
	{
		GridView grid;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_image_list);

			var toolbar = FindViewById<Toolbar> (Resource.Id.toolbar);

			SetSupportActionBar (toolbar);
			SupportActionBar.Title = "Albums";

			grid = FindViewById<GridView> (Resource.Id.grid);

			grid.Adapter = new GridAdapter (this);
			grid.ItemClick += (sender, e) => {
				var intent = new Intent(this, typeof(ImageDetailPaletteActivity));
				intent.PutExtra("id", Photos.Items[e.Position].Id);
				StartActivity(intent);
			};

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
				convertView = context.LayoutInflater.Inflate (Resource.Layout.grid_item, null);

			var item = Photos.Items[position];
			convertView.FindViewById<ImageView> (Resource.Id.imageView).SetImageResource (item.Image);
			convertView.FindViewById<TextView> (Resource.Id.textView).Text = item.Name;

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

