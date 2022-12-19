using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Runtime;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using Java.Lang;
using TaskStackBuilder = AndroidX.Core.App.TaskStackBuilder;

namespace LocalNotifications;

[Activity (Label = "Notifications", MainLauncher = true, Icon = "@drawable/Icon")]
public class MainActivity : AppCompatActivity
{
    const string PERMISSION = "android.permission.POST_NOTIFICATIONS";
    const int REQUEST_CODE = 1;
    /// <summary>
    /// Unique ID for our notification
    /// </summary>
    const int NOTIFICATION_ID = 1000;
    const string CHANNEL_ID = "location_notification";
    public const string COUNT_KEY = "count";

    // Number of times the button is tapped (starts with first tap):
    int count = 1;

    protected override void OnCreate (Bundle? bundle)
    {
        base.OnCreate (bundle);
        SetContentView (Resource.Layout.Main);

        // Display the "Hello World, Click Me!" button and register its event handler:
        var button = RequireViewById<Button> (Resource.Id.MyButton);
        button.Click += (sender, e) =>
        {
            if (CheckSelfPermission(PERMISSION) == Permission.Denied)
            {
                ActivityCompat.RequestPermissions(this, new[] { PERMISSION }, REQUEST_CODE);
            }
            else
            {
                CreateNotification();
            }
        };

        CreateNotificationChannel();
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        if (requestCode == REQUEST_CODE)
        {
            if (permissions.Length > 0 && permissions[0] == PERMISSION &&
                grantResults.Length > 0 && grantResults[0] == Permission.Granted)
            {
                CreateNotification();
            }
            else
            {
                Toast.MakeText(this, "Unable to request permissions!", ToastLength.Long)!.Show();
            }
        }
    }

    void CreateNotification()
    {
        // Pass the current button press count value to the next activity:
        var valuesForActivity = new Bundle();
        valuesForActivity.PutInt(COUNT_KEY, count);

        // When the user clicks the notification, SecondActivity will start up.
        var resultIntent = new Intent(this, typeof(SecondActivity));

        // Pass some values to SecondActivity:
        resultIntent.PutExtras(valuesForActivity);

        // Construct a back stack for cross-task navigation:
        var stackBuilder = TaskStackBuilder.Create(this);
        ArgumentNullException.ThrowIfNull(stackBuilder);
        stackBuilder.AddParentStack(Class.FromType(typeof(SecondActivity)));
        stackBuilder.AddNextIntent(resultIntent);

        // Create the PendingIntent with the back stack:            
        var resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.Immutable);

        // Build the notification:
        var builder = new NotificationCompat.Builder(this, CHANNEL_ID)
                        .SetAutoCancel(true) // Dismiss the notification from the notification area when the user clicks on it
                        .SetContentIntent(resultPendingIntent) // Start up this activity when the user clicks the intent.
                        .SetContentTitle("Button Clicked") // Set the title
                        .SetNumber(count) // Display the count in the Content Info
                        .SetSmallIcon(Resource.Mipmap.ic_stat_button_click) // This is the icon to display
                        .SetContentText($"The button has been clicked {count} times."); // the message to display.

        // Finally, publish the notification:
        var notificationManager = NotificationManagerCompat.From(this);
        if (notificationManager.AreNotificationsEnabled())
        {
            notificationManager.Notify(NOTIFICATION_ID, builder.Build());
        }
        else
        {
            Toast.MakeText(this, "Notifications are not enabled!", ToastLength.Long)!.Show();
        }

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

            if (GetSystemService(NotificationService) is NotificationManager manager)
            {
                manager.CreateNotificationChannel(channel);
            }
        }
    }
}
