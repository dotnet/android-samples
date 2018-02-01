using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;

namespace AutofillFramework
{
    [Activity(Label = "EmailComposeActivity")]
    [Register("com.xamarin.AutofillFramework.EmailComposeActivity")]
    public class EmailComposeActivity : AppCompatActivity
    {
        public static Intent GetStartActivityIntent(Context context)
        {
            var intent = new Intent(context, typeof(EmailComposeActivity));
            return intent;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.email_compose_activity);
            FindViewById(Resource.Id.sendButton).Click += delegate
            {
                StartActivity(WelcomeActivity.GetStartActivityIntent(this));
                Finish();
            };
        }
    }
}