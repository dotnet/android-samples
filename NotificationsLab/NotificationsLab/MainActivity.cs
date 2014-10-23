using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Media;
using System.Threading;

namespace NotificationsLab
{
    [Activity(Label = "Notifications Lab", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Material")]
    public class MainActivity : Activity
    {
		EditText notifyMsg;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set the view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get the notifications manager:
            NotificationManager notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;

            // Setup an intent so that notifications can return to this app:
            var intent = new Intent (this, typeof(MainActivity));
            var pendingIntent = PendingIntent.GetActivity (this, 0, intent, PendingIntentFlags.CancelCurrent);

            //..........................................................................
            // Edit box:

            // Find the notification edit text box in the layout:
            notifyMsg = FindViewById<EditText>(Resource.Id.notifyText);

            //..........................................................................
            // Selection Spinners:
            // The spinners in this file use the strings defined in Resources/values/arrays.xml.

            // Find the Style spinner in the layout and configure its adapter.
            Spinner styleSpinner = FindViewById<Spinner>(Resource.Id.styleSpinner);
            var styleAdapter = ArrayAdapter.CreateFromResource(this,
                                    Resource.Array.notification_style, 
                                    Android.Resource.Layout.SimpleSpinnerDropDownItem);
            styleSpinner.Adapter = styleAdapter;

			// Handler for Style spinner, changes the text in the text box:
			styleSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (styleSpinnerSelected);

            // Find the Visibility spinner in the layout and configure its adapter:
            Spinner visibilitySpinner = FindViewById<Spinner>(Resource.Id.visibilitySpinner);
            var visibilityAdapter = ArrayAdapter.CreateFromResource(this,
                                        Resource.Array.notification_visibility, 
                                        Android.Resource.Layout.SimpleSpinnerDropDownItem);
            visibilitySpinner.Adapter = visibilityAdapter;

            // Find the Priority spinner in the layout and configure its adapter:
            Spinner prioritySpinner = FindViewById<Spinner>(Resource.Id.prioritySpinner);
            var priorityAdapter = ArrayAdapter.CreateFromResource(this,
                                        Resource.Array.notification_priority, 
                                        Android.Resource.Layout.SimpleSpinnerDropDownItem);
            prioritySpinner.Adapter = priorityAdapter;

            // Find the Category spinner in the layout and configure its adapter:
            Spinner categorySpinner = FindViewById<Spinner>(Resource.Id.categorySpinner);
            var categoryAdapter = ArrayAdapter.CreateFromResource(this,
                                        Resource.Array.notification_category,
                                        Android.Resource.Layout.SimpleSpinnerDropDownItem);
            categorySpinner.Adapter = categoryAdapter;

            // Handler for Style spinner: changes the text in the text box:
            styleSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (styleSpinnerSelected);

            //..........................................................................
            // Option Switches:

            // Get large-icon, sound, and vibrate switches from the layout:
            Switch largeIconSw = FindViewById<Switch>(Resource.Id.largeIconSwitch);
            Switch soundSw = FindViewById<Switch>(Resource.Id.soundSwitch);
            Switch vibrateSw = FindViewById<Switch>(Resource.Id.vibrateSwitch);

            //..........................................................................
            // Notification Launch button:

            // Get notification launch button from the layout:
            Button launchBtn = FindViewById<Button>(Resource.Id.launchButton);

            // Handler for the notification launch button. When this button is clicked, this 
            // handler code takes the following steps, in order:
            //
            //  1. Instantiates the builder.
            //  2. Calls methods on the builder to optionally plug in the large icon, extend
            //     the style (if called for by a spinner selection), set the visibility, set 
            //     the priority, and set the category. 
            //  3. Uses the builder to instantiate the notification.
            //  4. Turns on sound and vibrate (if selected).
            //  5. Uses the Notification Manager to launch the notification.

            launchBtn.Click += delegate
            {
                // Instantiate the notification builder:
				Notification.Builder builder = new Notification.Builder(this)
                    .SetContentIntent(pendingIntent)
					.SetContentTitle("Sample Notification")
                    .SetContentText(notifyMsg.Text)
                    .SetSmallIcon(Resource.Drawable.ic_notification)
                    .SetAutoCancel(true);

                // Add large icon if selected:
                if (largeIconSw.Checked)
                    builder.SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.monkey_icon));

                // Extend style based on Style spinner selection.
                switch (styleSpinner.SelectedItem.ToString())
                {
                    case "Big Text":

                        // Extend the message with the big text format style. This will
                        // use a larger screen area to display the notification text.

                        // Set the title for demo purposes:
					    builder.SetContentTitle("Big Text Notification");

                        // Using the Big Text style:
                        var textStyle = new Notification.BigTextStyle();

                        // Use the text in the edit box at the top of the screen.
                        textStyle.BigText(notifyMsg.Text);
                        textStyle.SetSummaryText("The summary text goes here.");

                        // Plug this style into the builder:
                        builder.SetStyle(textStyle);
                        break;

                    case "Inbox":

                        // Present the notification in inbox format instead of normal text style.
                        // Note that this does not display the notification message entered
                        // in the edit text box; instead, it displays the fake email inbox
                        // summary constructed below.

                        // Using the inbox style:
                        var inboxStyle = new Notification.InboxStyle();

                        // Set the title of the notification:
                        builder.SetContentTitle("5 new messages");
                        builder.SetContentText("chimchim@xamarin.com");

                        // Generate inbox notification text:
                        inboxStyle.AddLine("Cheeta: Bananas on sale");
                        inboxStyle.AddLine("George: Curious about your blog post");
                        inboxStyle.AddLine("Nikko: Need a ride to Evolve?");
                        inboxStyle.SetSummaryText("+2 more");

                        // Plug this style into the builder:
                        builder.SetStyle(inboxStyle);
                        break;

                    case "Image":

                        // Extend the message with image (big picture) format style. This displays
                        // the Resources/drawables/x_bldg.jpg image in the notification body.

                        // Set the title for demo purposes:
					    builder.SetContentTitle("Image Notification");

                        // Using the Big Picture style:
                        var picStyle = new Notification.BigPictureStyle();

                        // Convert the image file to a bitmap before passing it into the style
                        // (there is no exception handler since we know the size of the image):
                        picStyle.BigPicture(BitmapFactory.DecodeResource(Resources, Resource.Drawable.x_bldg));
                        picStyle.SetSummaryText("The summary text goes here.");

                        // Alternately, uncomment this code to use an image from the SD card.
                        // (In production code, wrap DecodeFile in an exception handler in case
                        // the image is too large and throws an out of memory exception.):
                        // BitmapFactory.Options options = new BitmapFactory.Options();
                        // options.InSampleSize = 2;
                        // string imagePath = "/sdcard/Pictures/my-tshirt.jpg";
						// picStyle.BigPicture(BitmapFactory.DecodeFile(imagePath, options));
                        // picStyle.SetSummaryText("Check out my new T-shirt!");

                        // Plug this style into the builder:
                        builder.SetStyle(picStyle);
                        break;

                    default:
                        // Normal text notification is the default.
                        break;
                }

                // Set visibility based on Visibility spinner selection:
                switch (visibilitySpinner.SelectedItem.ToString())
                {
                    case "Public":
                        builder.SetVisibility(NotificationVisibility.Public);
                        break;
                    case "Private":
                        builder.SetVisibility(NotificationVisibility.Private);
                        break;
                    case "Secret":
                        builder.SetVisibility(NotificationVisibility.Secret);
                        break;
                }

                // Set priority based on Priority spinner selection:
                switch (prioritySpinner.SelectedItem.ToString())
                {
                    case "High":
					builder.SetPriority((int)NotificationPriority.High);
                        break;
                    case "Low":
                        builder.SetPriority((int)NotificationPriority.Low);
                        break;
                    case "Maximum":
                        builder.SetPriority((int)NotificationPriority.Max);
                        break;
                    case "Minimum":
                        builder.SetPriority((int)NotificationPriority.Min);
                        break;
                    default:
                        builder.SetPriority((int)NotificationPriority.Default);
                        break;
                }

                // Set category based on Category spinner selection:
                switch (categorySpinner.SelectedItem.ToString())
                {
                    case "Call":
                        builder.SetCategory(Notification.CategoryCall);
                        break;
                    case "Message":
                        builder.SetCategory(Notification.CategoryMessage);
                        break;
                    case "Alarm":
                        builder.SetCategory(Notification.CategoryAlarm);
                        break;
                    case "Email":
                        builder.SetCategory(Notification.CategoryEmail);
                        break;
                    case "Event":
                        builder.SetCategory(Notification.CategoryEvent);
                        break;
                    case "Promo":
                        builder.SetCategory(Notification.CategoryPromo);
                        break;
                    case "Progress":
                        builder.SetCategory(Notification.CategoryProgress);
                        break;
                    case "Social":
                        builder.SetCategory(Notification.CategorySocial);
                        break;
                    case "Error":
                        builder.SetCategory(Notification.CategoryError);
                        break;
                    case "Transport":
                        builder.SetCategory(Notification.CategoryTransport);
                        break;
                    case "System":
                        builder.SetCategory(Notification.CategorySystem);
                        break;
                    case "Service":
                        builder.SetCategory(Notification.CategoryService);
                        break;
                    case "Recommendation":
                        builder.SetCategory(Notification.CategoryRecommendation);
                        break;
                    case "Status":
                        builder.SetCategory(Notification.CategoryStatus);
                        break;
                    default:
                        builder.SetCategory(Notification.CategoryStatus);
                        break;
                }

                // Build the notification:
                Notification notification = builder.Build();
              
                // Turn on sound if the sound switch is on:
                if (soundSw.Checked)
                    notification.Defaults |= NotificationDefaults.Sound;

                // Turn on vibrate if the sound switch is on:
                if (vibrateSw.Checked)
                    notification.Defaults |= NotificationDefaults.Vibrate;

                // Notification ID used for all notifications in this app.
                // Reusing the notification ID prevents the creation of 
                // numerous different notifications as the user experiments
                // with different notification settings -- each launch reuses
                // and updates the same notification.
				const int notificationId = 1;

                // Launch notification:
                notificationManager.Notify(notificationId, notification);

                // Uncomment this code to update the notification 5 seconds later:
				// Thread.Sleep(5000);
				// builder.SetContentTitle("Updated Notification");
				// builder.SetContentText("Changed to this message after five seconds.");
				// notification = builder.Build();
				// notificationManager.Notify(notificationId, notification);
            };
        }

        // Handler for changes in style in the Style spinner. This handler updates
        // the notification text to reflect the chosen style.

		private void styleSpinnerSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinner = sender as Spinner;
			switch (spinner.SelectedItem.ToString())
			{
                case "Big Text":
                    notifyMsg.Text  = "Notification message text goes here. ";
                    notifyMsg.Text += "You can modify the message in this text box ";
                    notifyMsg.Text += "before tapping LAUNCH NOTIFICATION, or you can ";
                    notifyMsg.Text += "just use this default text.";
                    break;

                case "Big Picture":
                    notifyMsg.Text   = "This notification contains an image. ";
                    notifyMsg.Text  += "After launching the notification, you can drag ";
                    notifyMsg.Text  += "down on the notification to view the image.";
                    break;

                case "Inbox":
                    notifyMsg.Text   = "This notification contains an example email summary. ";
                    notifyMsg.Text  += "After launching the notification, you can drag ";
                    notifyMsg.Text  += "down on the notification to view the summary.";
                    break;

                default:
                    notifyMsg.Text  = "Notification message text goes here.";
                    break;
            }
		}
    }
}

