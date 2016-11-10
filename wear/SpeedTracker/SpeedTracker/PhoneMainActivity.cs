using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Text.Format;
using Android.Util;
using Android.Views;
using Java.Lang;
using Java.Util;

namespace SpeedTracker
{
	/**
	 * The main activity for the handset application. When a watch device reconnects to the handset
	 * app, the collected GPS data on the watch, if any, is synced up and user can see his/her track on
	 * a map. This data is then saved into an internal database and the corresponding data items are
	 * deleted.
	 */
	[Activity(Label = "SpeedTracker", MainLauncher = true, Icon = "@drawable/ic_launcher", Theme = "@style/Theme.Base", 
	          ScreenOrientation = ScreenOrientation.Portrait)]
	public class PhoneMainActivity : AppCompatActivity, DatePickerDialog.IOnDateSetListener, View.IOnClickListener
	{
		private static readonly string TAG = "PhoneMainActivity";
		private static readonly int BOUNDING_BOX_PADDING_PX = 50;
		private TextView mSelectedDateText;
		private GoogleMap mMap;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView (Resource.Layout.main_activity);

			mSelectedDateText = FindViewById<TextView>(Resource.Id.selected_date);
			mMap = ((MapFragment)FragmentManager.FindFragmentById(Resource.Id.map)).Map;
			FindViewById(Resource.Id.date_picker).SetOnClickListener(this);
		}

		public void OnClick(View view)
		{
			var calendar = Calendar.Instance;
			calendar.TimeInMillis = JavaSystem.CurrentTimeMillis();
			new DatePickerDialog(this, this,
				calendar.Get(CalendarField.Year),
				calendar.Get(CalendarField.Month), 
				calendar.Get(CalendarField.DayOfMonth))
				.Show();
		}

		public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
		{
			// the following if-clause is to get around a bug that causes this callback to be called
			// twice
			if (!view.IsShown) return;
			var calendar = Calendar.Instance;
			calendar.TimeInMillis = JavaSystem.CurrentTimeMillis();
			calendar.Set(CalendarField.Year, year);
			calendar.Set(CalendarField.Month, month);
			calendar.Set(CalendarField.DayOfMonth, dayOfMonth);
			var date = DateUtils.FormatDateTime(this, calendar.TimeInMillis, FormatStyleFlags.ShowDate);
			mSelectedDateText.Text = GetString(Resource.String.showing_for_date, date);
			ShowTrack(calendar);
		}

		/**
		 * An {@link android.os.AsyncTask} that is responsible for getting a list of {@link
		 * com.xamarin.android.wearable.speedtracker.common.LocationEntry} objects for a given day and
		 * building a track from those points. In addition, it sets the smallest bounding box for the
		 * map that covers all the points on the track.
		 */
		private void ShowTrack(Calendar calendar)
		{
			new ShowTrackTask((PhoneApplication) Application, this).Execute(calendar);
		}

		private class ShowTrackTask : AsyncTask<Calendar, Void, Void>
		{
			private ArrayList coordinates;
			private LatLngBounds bounds;
			private PhoneApplication App { get; set; }
			private PhoneMainActivity Owner { get; set; }

			public ShowTrackTask(PhoneApplication app, PhoneMainActivity owner)
			{
				App = app;
				Owner = owner;
			}

			protected override Void RunInBackground(params Calendar[] @params)
			{
				var dataManager = App.GetDataManager();
				var entries = dataManager.GetPoints(@params[0]);
				if (entries == null || entries.Count == 0) return null;
				coordinates = new ArrayList();
				var builder = new LatLngBounds.Builder();
				foreach (var entry in entries)
				{
					var latLng = new LatLng(entry.latitude, entry.longitude);
					builder.Include(latLng);
					coordinates.Add(latLng);
				}
				bounds = builder.Build();
				return null;
			}

			protected override void OnPostExecute(Object result)
			{
				Owner.mMap.Clear();
				if (coordinates == null || coordinates.IsEmpty)
				{
					if (Log.IsLoggable(TAG, LogPriority.Debug))
					{
						Log.Debug(TAG, "No Entries found for that date");
					}
				}
				else
				{
					Owner.mMap.MoveCamera(CameraUpdateFactory.NewLatLngBounds(bounds, BOUNDING_BOX_PADDING_PX));
					Owner.mMap.AddPolyline(new PolylineOptions().Geodesic(true).AddAll(coordinates));
				}
			}
		}
	}
}

