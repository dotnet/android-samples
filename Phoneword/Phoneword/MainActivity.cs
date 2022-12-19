using Core;

namespace Phoneword;

[Activity(Label = "Phone Word", MainLauncher = true, Icon = "@mipmap/ic_launcher")]
public class MainActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Set our view from the "main" layout resource
        SetContentView(Resource.Layout.Main);

        // Get our UI controls from the loaded layout
        EditText phoneNumberText = RequireViewById<EditText>(Resource.Id.PhoneNumberText);
        Button translateButton = RequireViewById<Button>(Resource.Id.TranslateButton);
        TextView translatedPhoneWord = RequireViewById<TextView>(Resource.Id.TranslatedPhoneWord);

        // Add code to translate number
        translateButton.Click += (sender, e) =>
        {
            // Translate user's alphanumeric phone number to numeric
            var translatedNumber = PhoneTranslator.ToNumber(phoneNumberText.Text);
            
            if (string.IsNullOrWhiteSpace(translatedNumber))
            {
                translatedPhoneWord.Text = string.Empty;
                Toast.MakeText(this, "Unable to translate number!", ToastLength.Long)!.Show();
            }
            else
            {
                translatedPhoneWord.Text = translatedNumber;
            }
        };
    }
}