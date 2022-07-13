using Android.Content;
using Android.Content.Res;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using Java.Lang;
using LocalNotifications;
using TaskStackBuilder = Android.App.TaskStackBuilder;

namespace Notifications;

[Activity (Label = "Notifications", MainLauncher = true, Icon = "@drawable/Icon")]
public class MainActivity : AppCompatActivity
{
    // Unique ID for our notification: 
    static readonly int NOTIFICATION_ID = 1000;
    static readonly string CHANNEL_ID = "location_notification";
    internal static readonly string COUNT_KEY = "count";

    // Number of times the button is tapped (starts with first tap):
    int count = 1;

    protected override void OnCreate (Bundle? bundle)
    {
        base.OnCreate (bundle);
        SetContentView (Resource.Layout.Main);

        CreateNotificationChannel ();

        // Display the "Hello World, Click Me!" button and register its event handler:
        var button = FindViewById<Button> (Resource.Id.MyButton);
        ArgumentNullException.ThrowIfNull (button);
        button.Click += ButtonOnClick;
    }

    // Handler for button click events.
    void ButtonOnClick (object? sender, EventArgs eventArgs)
    {
        // Pass the current button press count value to the next activity:
        var valuesForActivity = new Bundle ();
        valuesForActivity.PutInt (COUNT_KEY, count);

        // When the user clicks the notification, SecondActivity will start up.
        var resultIntent = new Intent (this, typeof (SecondActivity));

        // Pass some values to SecondActivity:
        resultIntent.PutExtras (valuesForActivity);

        // Construct a back stack for cross-task navigation:
        var stackBuilder = TaskStackBuilder.Create (this);
        ArgumentNullException.ThrowIfNull (stackBuilder);
        stackBuilder.AddParentStack (Class.FromType (typeof (SecondActivity)));
        stackBuilder.AddNextIntent (resultIntent);

        // Create the PendingIntent with the back stack:            
        var resultPendingIntent = stackBuilder.GetPendingIntent (0, PendingIntentFlags.Immutable);

        // Build the notification:
        var builder = new NotificationCompat.Builder (this, CHANNEL_ID)
                        .SetAutoCancel (true) // Dismiss the notification from the notification area when the user clicks on it
                        .SetContentIntent (resultPendingIntent) // Start up this activity when the user clicks the intent.
                        .SetContentTitle ("Button Clicked") // Set the title
                        .SetNumber (count) // Display the count in the Content Info
                        .SetSmallIcon (Resource.Mipmap.ic_stat_button_click) // This is the icon to display
                        .SetContentText ($"The button has been clicked {count} times."); // the message to display.

        // Finally, publish the notification:
        var notificationManager = NotificationManagerCompat.From (this);
        notificationManager.Notify (NOTIFICATION_ID, builder.Build ());

        // Increment the button press count:
        count++;
    }

    void CreateNotificationChannel ()
    {
        ArgumentNullException.ThrowIfNull (Resources);

        // Creating a NotificationChannel is only needed in API 26+
        if (OperatingSystem.IsAndroidVersionAtLeast (26))
        {
            var name = Resources.GetString (Resource.String.channel_name);
            var description = GetString (Resource.String.channel_description);
            var channel = new NotificationChannel (CHANNEL_ID, name, NotificationImportance.Default)
            {
                Description = description
            };

            var notificationManager = (NotificationManager?)GetSystemService (NotificationService);
            notificationManager?.CreateNotificationChannel (channel);
        }
    }
}
