using System;
using Android.App;
using Android.OS;
using Android.Widget;

namespace StandardControls {

    [Activity(Label = "TimePickerDialog")]
    public class TimePickerDialogScreen : Activity {

        private TextView time_display;
        private Button pick_button;

        private int hour;
        private int minute;

        const int TIME_DIALOG_ID = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "Main" layout resource  
            SetContentView(Resource.Layout.TimePickerDialog);

            // Capture our View elements  
            time_display = FindViewById<TextView>(Resource.Id.timeDisplay);
            pick_button = FindViewById<Button>(Resource.Id.pickTime);

            // Add a click listener to the button  
            pick_button.Click += (o, e) => ShowDialog(TIME_DIALOG_ID);

            // Get the current time  
            hour = DateTime.Now.Hour;
            minute = DateTime.Now.Minute;

            // Display the current date  
            UpdateDisplay();
        }

        // Updates the time we display in the TextView  
        private void UpdateDisplay()
        {
            string time = string.Format("{0}:{1}", hour, minute.ToString().PadLeft(2, '0'));
            time_display.Text = time;
        }

        private void TimePickerCallback(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            hour = e.HourOfDay;
            minute = e.Minute;
            UpdateDisplay();
        }

        protected override Dialog OnCreateDialog(int id)
        {
            if (id == TIME_DIALOG_ID)
                return new TimePickerDialog(this, TimePickerCallback, hour, minute, false);

            return null;
        }
    }
}

