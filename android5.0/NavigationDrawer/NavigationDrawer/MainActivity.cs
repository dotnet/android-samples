
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

namespace NavigationDrawer
{
	[Activity (Label = "@string/app_name", MainLauncher = true)]			
	public class MainActivity : Activity, AdapterView.IOnItemClickListener
	{
		internal Sample[] mSamples;
		internal GridView mGridView;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.activity_main);

			// Prepare list of samples in this dashboard.
			mSamples = new Sample[] {
				new Sample (Resource.String.navigationdraweractivity_title, 
					Resource.String.navigationdraweractivity_description,
					this,
					typeof(NavigationDrawerActivity)),
			};

			// Prepare the GridView
			mGridView = FindViewById<GridView> (Android.Resource.Id.List);
			mGridView.Adapter = new SampleAdapter (this);
			mGridView.OnItemClickListener = this;
		}

		public void OnItemClick (AdapterView container, View view, int position, long id)
		{
			StartActivity (mSamples [position].intent);
		}
	}

	internal class SampleAdapter : BaseAdapter
	{
		private MainActivity owner;

		public SampleAdapter (MainActivity owner) : base ()
		{
			this.owner = owner;
		}

		public override int Count {
			get {
				return owner.mSamples.Length;
			}
		}


		public override Java.Lang.Object GetItem (int position)
		{
			return owner.mSamples [position];
		}

		public override long GetItemId (int position)
		{
			return (long)owner.mSamples [position].GetHashCode ();
		}

		public override View GetView (int position, View convertView, ViewGroup container)
		{
			if (convertView == null) {
				convertView = owner.LayoutInflater.Inflate (Resource.Layout.sample_dashboard_item, container, false);
			}
			convertView.FindViewById<TextView> (Android.Resource.Id.Text1).SetText (owner.mSamples [position].titleResId);
			convertView.FindViewById<TextView> (Android.Resource.Id.Text2).SetText (owner.mSamples [position].descriptionResId);
			return convertView;
		}
	}

	internal class Sample : Java.Lang.Object
	{
		internal int titleResId;
		internal int descriptionResId;
		internal Intent intent;

		public Sample (int titleResId, int descriptionResId, Intent intent)
		{
			Initialize (titleResId, descriptionResId, intent);
		}

		public Sample (int titleResId, int descriptionResId, Context c, Type t)
		{
			Initialize (titleResId, descriptionResId, new Intent (c, t));
		}

		private void Initialize (int titleResId, int descriptionResId, Intent intent)
		{
			this.intent = intent;
			this.titleResId = titleResId;
			this.descriptionResId = descriptionResId;
		}
	}
}

