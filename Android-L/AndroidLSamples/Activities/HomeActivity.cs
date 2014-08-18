
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

namespace AndroidLSamples
{
	public class ActivityItem
	{
		public string Title { get; set; }
		public Intent Intent { get; set; }

		public override string ToString ()
		{
			return Title;
		}
	}

	[Activity (Label = "Android L Samples", MainLauncher = true)]			
	public class HomeActivity : ListActivity
	{

		private ActivityItem[] Activities;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Activities = new [] {
				new ActivityItem{ Title = "Material Theme Colors - v21", Intent = new Intent(this, typeof(ThemesActivity)) },
				new ActivityItem{	Title = "Elevation - v21", Intent = new Intent(this, typeof(ElevationActivity)) },
				new ActivityItem{	Title = "CardView - v7", Intent = new Intent(this, typeof(CardActivity)) },
				new ActivityItem{ Title = "RecyclerView - v7",	Intent = new Intent(this, typeof(RecyclerViewActivity)) },
				new ActivityItem{ Title = "RecyclerView Add/Remove - v7",	Intent = new Intent(this, typeof(RecyclerViewActivityAddRemove)) },
				new ActivityItem{ Title = "Palette - v7", Intent = new Intent(this, typeof(ImageListActivity))},
				new ActivityItem{ Title = "Animations - Explode - v21",	Intent = new Intent(this, typeof(AnimationsActivity1))},
				new ActivityItem{ Title = "Animations - Move Image - v21",	Intent = new Intent(this, typeof(AnimationsActivityMoveImage1))},
				new ActivityItem{ Title = "Notifications - v21", Intent = new Intent(this, typeof(NotificationsActivity)) }
			};

			ListAdapter = new ArrayAdapter<ActivityItem> (this, Android.Resource.Layout.SimpleListItem1, Android.Resource.Id.Text1, Activities); 
		}

		protected override void OnListItemClick (ListView l, View v, int position, long id)
		{
			base.OnListItemClick (l, v, position, id);

			StartActivity (Activities [position].Intent);
		}
		public override bool OnCreateOptionsMenu (Android.Views.IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.menu1, menu);
			return base.OnCreateOptionsMenu (menu);
		}
	}
}

