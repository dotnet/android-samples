using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Autofill;
using Android.Widget;
using AndroidX.AppCompat.App;
using Java.Lang;

namespace AutofillFramework
{
    [Activity(Label = "CreditCardExpirationDatePickerView")]
    [Register("com.xamarin.AutofillFramework.CreditCardExpirationDatePickerView")]
    public class CreditCardDatePickerActivity : AppCompatActivity
    {
        private CreditCardExpirationDatePickerView mCcExpDateView;
        private EditText mCcExpNumber;
        private EditText mCcSecurityCode;

        public static Intent GetStartActivityIntent(Context context)
        {
            var intent = new Intent(context, typeof(CreditCardDatePickerActivity));
            return intent;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.credit_card_date_picker_activity);
            mCcExpDateView = FindViewById<CreditCardExpirationDatePickerView>(Resource.Id.creditCardExpirationView);
            mCcExpNumber = FindViewById<EditText>(Resource.Id.creditCardNumberField);
            mCcSecurityCode = FindViewById<EditText>(Resource.Id.creditCardSecurityCode);
            FindViewById(Resource.Id.submitButton).Click += delegate { Submit(); };
            FindViewById(Resource.Id.clearButton).Click += delegate
            {
                ((AutofillManager) GetSystemService(Class.FromType(typeof(AutofillManager)))).Cancel();
                ResetFields();
            };

            mCcExpDateView.Reset();
        }

        private void ResetFields()
        {
            mCcExpDateView.Reset();
            mCcExpNumber.Text = string.Empty;
            mCcSecurityCode.Text = string.Empty;
        }

        public void ShowDatePickerDialog(View v)
        {
            if (v != mCcExpDateView)
            {
                Log.Warn(CommonUtil.TAG, "showDatePickerDialog() called on invalid view: " + v);
                return;
            }

            mCcExpDateView.ShowDatePickerDialog(SupportFragmentManager);
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