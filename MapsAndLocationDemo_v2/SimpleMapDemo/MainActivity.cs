namespace SimpleMapDemo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Android.App;
    using Android.Content;
	using Android.Gms.Common;
    using Android.OS;
	using Android.Util;
    using Android.Views;
    using Android.Widget;

    using AndroidUri = Android.Net.Uri;

    [Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : ListActivity
    {
		public static readonly int InstallGooglePlayServicesId = 1000;

        private List<SampleActivity> _activities;
		private bool _isGooglePlayServicesInstalled;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
			_isGooglePlayServicesInstalled = TestIfGooglePlayServicesIsInstalled ();
			InitializeListView ();
        }

		private void InitializeListView()
		{
			if (_isGooglePlayServicesInstalled) 
			{
				_activities = new List<SampleActivity>
				{
					new SampleActivity(Resource.String.mapsAppText, Resource.String.mapsAppTextDescription, null),
					new SampleActivity(Resource.String.basic_map, Resource.String.basic_map_description, typeof(BasicDemoActivity)),
					new SampleActivity(Resource.String.activity_label_samplemap, Resource.String.showMapActivityDescription, typeof(MapWithMarkersActivity)),
					new SampleActivity(Resource.String.activity_label_mapwithmarkers, Resource.String.showMapWithOverlaysDescription, typeof(SampleMapActivity))
				};

				ListAdapter = new SimpleMapDemoActivityAdapter(this, _activities);
			}
			else
			{
				Log.Error ("MainActivity", "Google Play Services is not installed");
				ListAdapter = new SimpleMapDemoActivityAdapter (this, null);
			}
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			switch (resultCode) 
			{
			case Result.Ok:
				// Try again.
				_isGooglePlayServicesInstalled = true;
				break;

			default:
				Log.Debug ("MainActivity", "Unknown resultCode {0} for request {1}", resultCode, requestCode);
				break;
			}
		}

		private bool TestIfGooglePlayServicesIsInstalled()
		{
			int queryResult = GooglePlayServicesUtil.IsGooglePlayServicesAvailable (this);
			if (queryResult == ConnectionResult.Success)
			{
				Log.Info ("SimpleMapDemo", "Google Play Services is installed on this device.");
				return true;
			}

			if (GooglePlayServicesUtil.IsUserRecoverableError (queryResult)) 
			{
				string errorString = GooglePlayServicesUtil.GetErrorString (queryResult);
				Log.Error ("SimpleMapDemo", "There is a problem with Google Play Services on this device: {0} - {1}", queryResult, errorString);
				Dialog errorDialog = GooglePlayServicesUtil.GetErrorDialog(queryResult, this, InstallGooglePlayServicesId);
				ErrorDialogFragment dialogFrag = new ErrorDialogFragment(errorDialog);

				dialogFrag.Show (FragmentManager, "GooglePlayServicesDialog");
			}
			return false;
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

	internal class ErrorDialogFragment :DialogFragment
	{
		public ErrorDialogFragment(Dialog dialog) 
		{
			Dialog = dialog;
		}

		public Dialog Dialog { get; private set; }

		public override Dialog OnCreateDialog (Bundle savedInstanceState)
		{
			return Dialog;

		}


	}

    internal class SimpleMapDemoActivityAdapter : BaseAdapter<SampleActivity>
    {
        private readonly List<SampleActivity> _activities;
        private readonly Context _context;

        public SimpleMapDemoActivityAdapter(Context context, IEnumerable<SampleActivity> sampleActivities)
        {
            _context = context;
			if (sampleActivities == null)
			{
				_activities = new List<SampleActivity>(0);
			}
			else 
			{
            	_activities = sampleActivities.ToList();
			}
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
