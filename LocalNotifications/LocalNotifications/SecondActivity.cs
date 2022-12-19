﻿using AndroidX.AppCompat.App;

namespace LocalNotifications;

[Activity (Label = "Second Activity")]
public class SecondActivity : AppCompatActivity
{
    protected override void OnCreate (Bundle? bundle)
    {
        base.OnCreate (bundle);

        // Get the count value passed to us from MainActivity:
        var count = Intent?.Extras?.GetInt (MainActivity.COUNT_KEY, -1);

        // No count was passed? Then just return.
        if (count is not null && count <= 0)
            return;

        // Display the count sent from the first activity:
        SetContentView (Resource.Layout.Second);
        var txtView = RequireViewById<TextView> (Resource.Id.textView1);
        txtView.Text = $"You clicked the button {count} times in the previous activity.";
    }
}
