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
    [Activity(Label = "CreditCardActivity")]
    [Register("com.xamarin.AutofillFramework.CreditCardActivity")]
    public class CreditCardActivity : AppCompatActivity
    {
        private EditText mCcExpDayView;
        private EditText mCcExpMonthView;
        private EditText mCcExpYearView;
        private EditText mCcNumber;
        private EditText mCcSecurityCode;

        public static Intent GetStartActivityIntent(Context context)
        {
            var intent = new Intent(context, typeof(CreditCardActivity));
            return intent;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.credit_card_activity);
            mCcExpDayView = FindViewById<EditText>(Resource.Id.expirationDay);
            mCcExpMonthView = FindViewById<EditText>(Resource.Id.expirationMonth);
            mCcExpYearView = FindViewById<EditText>(Resource.Id.expirationYear);
            mCcNumber = FindViewById<EditText>(Resource.Id.creditCardNumberField);
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
            mCcExpDayView.Text = string.Empty;
            mCcExpMonthView.Text = string.Empty;
            mCcExpYearView.Text = string.Empty;
            mCcNumber.Text = string.Empty;
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