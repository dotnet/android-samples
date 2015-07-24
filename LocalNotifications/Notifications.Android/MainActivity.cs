using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Java.Lang;
using String = System.String;
using NotificationCompat = Android.Support.V4.App.NotificationCompat;
using TaskStackBuilder = Android.Support.V4.App.TaskStackBuilder;

namespace Notifications
{
    [Activity(Label = "Notifications", MainLauncher = true, Icon = "@drawable/Icon")]
    public class MainActivity : Activity
    {
        // Unique ID for our notification: 
        private static readonly int ButtonClickNotificationId = 1000;

        // Number of times the button is tapped (starts with first tap):
        private int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            // Display the "Hello World, Click Me!" button and register its event handler:
            Button button = FindViewById<Button>(Resource.Id.MyButton);
            button.Click += ButtonOnClick;
        }

        // Handler for button click events.
        private void ButtonOnClick(object sender, EventArgs eventArgs)
        {
            // Pass the current button press count value to the next activity:
            Bundle valuesForActivity = new Bundle();
            valuesForActivity.PutInt("count", count);

            // When the user clicks the notification, SecondActivity will start up.
            Intent resultIntent = new Intent(this, typeof(SecondActivity));

            // Pass some values to SecondActivity:
            resultIntent.PutExtras(valuesForActivity); 

            // Construct a back stack for cross-task navigation:
            TaskStackBuilder stackBuilder = TaskStackBuilder.Create(this);
            stackBuilder.AddParentStack(Class.FromType(typeof(SecondActivity)));
            stackBuilder.AddNextIntent(resultIntent);

            // Create the PendingIntent with the back stack:            
            PendingIntent resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

            // Build the notification:
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
                .SetAutoCancel(true)                    // Dismiss the notification from the notification area when the user clicks on it
                .SetContentIntent(resultPendingIntent)  // Start up this activity when the user clicks the intent.
                .SetContentTitle("Button Clicked")      // Set the title
                .SetNumber(count)                       // Display the count in the Content Info
                .SetSmallIcon(Resource.Drawable.ic_stat_button_click) // This is the icon to display
                .SetContentText(String.Format("The button has been clicked {0} times.", count)); // the message to display.

            // Finally, publish the notification:
            NotificationManager notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Notify(ButtonClickNotificationId, builder.Build());

            // Increment the button press count:
            count++;
        }
    }
}
