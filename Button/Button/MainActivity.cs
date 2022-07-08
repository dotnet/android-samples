namespace Mono.Samples.Button;

[Activity (Label = "Button Demo", MainLauncher = true)]
public class ButtonActivity : Activity
{
    int count = 0;

    protected override void OnCreate (Bundle? bundle)
    {
        base.OnCreate (bundle);

        // Create your application here
        Android.Widget.Button button = new (this);

        button.Text = $"{count} clicks!!";
        button.Click += delegate { 
            button.Text = string.Format ("{0} clicks!!", ++count); 
        };

        SetContentView (button);
    }
}
