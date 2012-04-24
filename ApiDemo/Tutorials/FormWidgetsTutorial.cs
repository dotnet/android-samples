using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Views;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Tutorials/Form Widgets")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class FormWidgetsTutorial : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.FormWidgetsTutorial);

			Button button = FindViewById<Button> (Resource.Id.button);

			button.Click += (o, e) => {
				Toast.MakeText (this, "Beep Boop", ToastLength.Short).Show ();
			};

			EditText edittext = FindViewById<EditText> (Resource.Id.edittext);

			edittext.KeyPress += (o, e) => {
				if (e.Event.Action == KeyEventActions.Down && ((Keycode)e.KeyCode) == Keycode.Enter) {
					Toast.MakeText (this, edittext.Text, ToastLength.Short).Show ();
				}

				e.Handled = false;
			};

			CheckBox checkbox = FindViewById<CheckBox> (Resource.Id.checkbox);

			checkbox.Click += (o, e) => {
				if (checkbox.Checked)
					Toast.MakeText (this, "Selected", ToastLength.Short).Show ();
				else
					Toast.MakeText (this, "Not selected", ToastLength.Short).Show ();
			};

			RadioButton radio_red = FindViewById<RadioButton> (Resource.Id.radio_red);
			RadioButton radio_blue = FindViewById<RadioButton> (Resource.Id.radio_blue);

			radio_red.Click += RadioButtonClick;
			radio_red.Click += RadioButtonClick;

			ToggleButton togglebutton = FindViewById<ToggleButton> (Resource.Id.togglebutton);

			togglebutton.Click += (o, e) => {
				// Perform action on clicks  
				if (togglebutton.Checked)
					Toast.MakeText (this, "Checked", ToastLength.Short).Show ();
				else
					Toast.MakeText (this, "Not checked", ToastLength.Short).Show ();
			};

			RatingBar ratingbar = FindViewById<RatingBar> (Resource.Id.ratingbar);

			ratingbar.RatingBarChange += (o, e) => {
				Toast.MakeText (this, "New Rating: " + ratingbar.Rating.ToString (), ToastLength.Short).Show ();
			};  
		}

		private void RadioButtonClick (object sender, EventArgs e)
		{
			RadioButton rb = (RadioButton)sender;
			Toast.MakeText (this, rb.Text, ToastLength.Short).Show ();
		}
	}
}
