using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views.Autofill;
using Android.Widget;
using AndroidX.AppCompat.App;
using Java.Lang;

namespace AutofillFramework
{
    [Activity(Label = "CreditCardAntiPatternActivity")]
    [Register("com.xamarin.AutofillFramework.CreditCardAntiPatternActivity")]
    public class CreditCardAntiPatternActivity : AppCompatActivity
    {
        private EditText mCcExpDateView;
        private EditText mCcExpNumber;
        private EditText mCcSecurityCode;

        public static Intent GetStartActivityIntent(Context context)
        {
            Intent intent = new Intent(context, typeof(CreditCardAntiPatternActivity));
            return intent;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.credit_card_anti_pattern_activity);
            mCcExpDateView = FindViewById<EditText>(Resource.Id.creditCardExpirationView);
            mCcExpNumber = FindViewById<EditText>(Resource.Id.creditCardNumberField);
            mCcSecurityCode = FindViewById<EditText>(Resource.Id.creditCardSecurityCode);
            FindViewById(Resource.Id.submitButton).Click += delegate { Submit(); };
            FindViewById(Resource.Id.clearButton).Click += delegate
            {
                ((AutofillManager) GetSystemService(Class.FromType(typeof(AutofillManager)))).Cancel();
                ResetFields();
            };
        }

        private void ResetFields()
        {
            mCcExpDateView.Text = string.Empty;
            mCcExpNumber.Text = string.Empty;
            mCcSecurityCode.Text = string.Empty;
        }

        /**
         * Launches new Activity and finishes, triggering an autofill save request if the user entered
         * any new data.
         */
        private void Submit()
        {
            var intent = WelcomeActivity.GetStartActivityIntent(this);
            StartActivity(intent);
            Finish();
        }
    }
}