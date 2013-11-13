namespace SimpleMapDemo
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Android.App;
	using Android.Content;
	using Android.OS;
	using Android.Views;
	using Android.Widget;

	using AndroidUri = Android.Net.Uri;

	[Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : ListActivity
	{
		private List<SampleActivity> _activities;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			_activities = new List<SampleActivity>
			{
				new SampleActivity(Resource.String.mapsAppText, Resource.String.mapsAppTextDescription, null),
				new SampleActivity(Resource.String.basic_map, Resource.String.basic_map_description, typeof(BasicDemoActivity)),
				new SampleActivity(Resource.String.activity_label_samplemap, Resource.String.showMapActivityDescription, typeof(MapWithMarkersActivity)),
				new SampleActivity(Resource.String.activity_label_mapwithmarkers, Resource.String.showMapWithOverlaysDescription, typeof(SampleMapActivity))
			};

			ListAdapter = new SimpleMapDemoActivityAdapter(this, _activities);
		}

		protected override void OnListItemClick(ListView l, View v, int position, long id)
		{
			if (position == 0)
			{
				AndroidUri geoUri = AndroidUri.Parse("geo:42.374260,-71.120824");
				Intent mapIntent = new Intent(Intent.ActionView, geoUri);
				StartActivity(mapIntent);
				return;
			}

			SampleActivity activity = _activities[position];
			activity.Start(this);
		}
	}

	internal class SimpleMapDemoActivityAdapter : BaseAdapter<SampleActivity>
	{
		private readonly List<SampleActivity> _activities;
		private readonly Context _context;

		public SimpleMapDemoActivityAdapter(Context context, IEnumerable<SampleActivity> sampleActivities)
		{
			_context = context;
			_activities = sampleActivities.ToList();
		}

		public override int Count { get { return _activities.Count; } }

		public override SampleActivity this[int position] { get { return _activities[position]; } }

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			FeatureRowHolder row = convertView as FeatureRowHolder ?? new FeatureRowHolder(_context);
			SampleActivity sample = _activities[position];

			row.UpdateFrom(sample);
			return row;
		}
	}

	internal class SampleActivity
	{
		public SampleActivity(int titleResourceId, int descriptionId, Type activityToLaunch)
		{
			ActivityToLaunch = activityToLaunch;
			TitleResource = titleResourceId;
			DescriptionResource = descriptionId;
		}

		public Type ActivityToLaunch { get; private set; }
		public int DescriptionResource { get; private set; }
		public int TitleResource { get; private set; }

		public void Start(Activity context)
		{
			Intent i = new Intent(context, ActivityToLaunch);
			context.StartActivity(i);
		}
	}

	internal class FeatureRowHolder : FrameLayout
	{
		private readonly TextView _description;
		private readonly TextView _title;

		public FeatureRowHolder(Context context)
			: base(context)
		{
			LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
			View view = inflater.Inflate(Resource.Layout.Feature, this);
			_title = view.FindViewById<TextView>(Resource.Id.title);
			_description = view.FindViewById<TextView>(Resource.Id.description);
		}

		public void UpdateFrom(SampleActivity sample)
		{
			_title.SetText(sample.TitleResource);
			_description.SetText(sample.DescriptionResource);
		}
	}
}
