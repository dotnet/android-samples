using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Util;
using Android.Text.Format;

namespace TimePickerDemo
{
    [Activity(Label = "TimePickerDemo", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        // TextView used to display the time selected in the TimePicker:
        TextView timeDisplay;

        // Button to launch TimePicker dialog:
        Button timeSelectButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            // Get views from the layout resource:
            timeDisplay = FindViewById<TextView>(Resource.Id.time_display);
            timeSelectButton = FindViewById<Button>(Resource.Id.select_button);

            // Attach an event to the button that launches the dialog:
            timeSelectButton.Click += TimeSelectOnClick;
        }

        // Handler for the button click
        void TimeSelectOnClick (object sender, EventArgs eventArgs)
        {
            // Instantiate a TimePickerFragment (defined below) 
            TimePickerFragment frag = TimePickerFragment.NewInstance (

                // Create and pass in a delegate that updates the Activity time display 
                // with the passed-in time value:
                delegate (DateTime time)
                {
                    timeDisplay.Text = time.ToShortTimeString();
                });

            // Launch the TimePicker dialog fragment (defined below):
            frag.Show(FragmentManager, TimePickerFragment.TAG);
        }
    }

    // TimePicker dialog fragment
    public class TimePickerFragment : DialogFragment, TimePickerDialog.IOnTimeSetListener
    {
        // TAG used for logging
        public static readonly string TAG = "MyTimePickerFragment";

        // Initialize handler to an empty delegate to prevent null reference exceptions:
        Action<DateTime> timeSelectedHandler = delegate { };

        // Factory method used to create a new TimePickerFragment:
        public static TimePickerFragment NewInstance(Action<DateTime> onTimeSelected)
        {
            // Instantiate a new TimePickerFragment:
            TimePickerFragment frag = new TimePickerFragment();

            // Set its event handler to the passed-in delegate:
            frag.timeSelectedHandler = onTimeSelected;

            // Return the new TimePickerFragment:
            return frag;
        }

        // Create and return a TimePickerDemo:
        public override Dialog OnCreateDialog (Bundle savedInstanceState)
        {
            // Get the current time
            DateTime currentTime = DateTime.Now;

            // Determine whether this activity uses 24-hour time format or not:
            bool is24HourFormat = DateFormat.Is24HourFormat(Activity);

            // Uncomment to force 24-hour time format:
            // is24HourFormat = true;

            // Instantiate a new TimePickerDemo, passing in the handler, the current 
            // time to display, and whether or not to use 24 hour format:
            TimePickerDialog dialog = new TimePickerDialog
                (Activity, this, currentTime.Hour, currentTime.Minute, is24HourFormat);
       
            // Return the created TimePickerDemo:
            return dialog;
        }

        // Called when the user sets the time in the TimePicker: 
        public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
        {
            // Get the current time:
            DateTime currentTime = DateTime.Now;

            // Create a DateTime that contains today's date and the time selected by the user:
            DateTime selectedTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, hourOfDay, minute, 0);

            // Log the date and selected time:
            Log.Debug(TAG, selectedTime.ToLongTimeString());

            // Invoke the handler to update the Activity's time display to the selected time:
            timeSelectedHandler (selectedTime);
        }
    }
}

