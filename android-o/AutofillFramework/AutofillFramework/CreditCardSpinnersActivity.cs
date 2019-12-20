using System;
using System.Collections;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views.Autofill;
using Android.Widget;
using AndroidX.AppCompat.App;
using Java.Lang;
using Object = Java.Lang.Object;

namespace AutofillFramework
{
    [Activity(Label = "CreditCardSpinnersActivity")]
    [Register("com.xamarin.AutofillFramework.CreditCardSpinnersActivity")]
    public class CreditCardSpinnersActivity : AppCompatActivity
    {
        private static int CC_EXP_YEARS_COUNT = 5;

        private string[] years = new string[CC_EXP_YEARS_COUNT];

        private Spinner mCcExpirationDaySpinner;
        private Spinner mCcExpirationMonthSpinner;
        private Spinner mCcExpirationYearSpinner;
        private EditText mCcCardNumber;
        private EditText mCcSecurityCode;

        public static Intent GetStartActivityIntent(Context context)
        {
            var intent = new Intent(context, typeof(CreditCardSpinnersActivity));
            return intent;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.credit_card_spinners_activity);
            mCcExpirationDaySpinner = FindViewById<Spinner>(Resource.Id.expirationDay);
            mCcExpirationMonthSpinner = FindViewById<Spinner>(Resource.Id.expirationMonth);
            mCcExpirationYearSpinner = FindViewById<Spinner>(Resource.Id.expirationYear);
            mCcCardNumber = FindViewById<EditText>(Resource.Id.creditCardNumberField);
            mCcSecurityCode = FindViewById<EditText>(Resource.Id.creditCardSecurityCode);

            // Create an ArrayAdapter using the string array and a default spinner layout
            var dayAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.day_array,
                Android.Resource.Layout.SimpleSpinnerItem);
            // Specify the layout to use when the list of choices appears
            dayAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            // Apply the adapter to the spinner
            mCcExpirationDaySpinner.Adapter = dayAdapter;

            /*
            R.array.month_array could be an array of Strings like "Jan", "Feb", "March", etc., and
            the AutofillService would know how to autofill it. However, for the sake of keeping the
            AutofillService simple, we will stick to a list of numbers (1, 2, ... 12) to represent
            months; it makes it much easier to generate fake autofill data in the service that can still
            autofill this spinner.
            */
            var monthAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.month_array,
                Android.Resource.Layout.SimpleSpinnerItem);
            // Adapter created from resource has getAutofillOptions() implemented by default.
            monthAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            mCcExpirationMonthSpinner.Adapter = monthAdapter;

            int year = Java.Util.Calendar.Instance.Get(Java.Util.Calendar.Year);
            for (int i = 0; i < years.Length; i++)
            {
                years[i] = (year + i).ToString();
            }

            // Since the years Spinner uses a custom adapter, it needs to implement getAutofillOptions.
            mCcExpirationYearSpinner.Adapter = new YearSpinnerArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, years) {that = this};

            FindViewById(Resource.Id.submit).Click += delegate { Submit(); };
            FindViewById(Resource.Id.clear).Click += delegate
            {
                ((AutofillManager) GetSystemService(Class.FromType(typeof(AutofillManager)))).Cancel();
                ResetFields();
            };
        }

        public class YearSpinnerArrayAdapter : ArrayAdapter
        {
            public CreditCardSpinnersActivity that;

            protected YearSpinnerArrayAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference,
                transfer)
            {
            }

            public YearSpinnerArrayAdapter(Context context, int resource) : base(context, resource)
            {
            }

            public YearSpinnerArrayAdapter(Context context, int resource, int textViewResourceId) : base(context,
                resource, textViewResourceId)
            {
            }

            public YearSpinnerArrayAdapter(Context context, int resource, int textViewResourceId, IList objects) : base(
                context, resource, textViewResourceId, objects)
            {
            }

            public YearSpinnerArrayAdapter(Context context, int resource, int textViewResourceId, Object[] objects) :
                base(context, resource, textViewResourceId, objects)
            {
            }

            public YearSpinnerArrayAdapter(Context context, int resource, IList objects) : base(context, resource,
                objects)
            {
            }

            public YearSpinnerArrayAdapter(Context context, int resource, Object[] objects) : base(context, resource,
                objects)
            {
            }

            public override ICharSequence[] GetAutofillOptionsFormatted()
            {
                return that.years.Select(x => new Java.Lang.String(x)).ToArray();
            }
        }

        private void ResetFields()
        {
            mCcExpirationDaySpinner.SetSelection(0);
            mCcExpirationMonthSpinner.SetSelection(0);
            mCcExpirationYearSpinner.SetSelection(0);
            mCcCardNumber.Text = string.Empty;
            mCcSecurityCode.Text = string.Empty;
        }

        /**
         * Launches new Activity and finishes, triggering an autofill save request if the user entered
         * any new data.
         */
        private void Submit()
        {
            Intent intent = WelcomeActivity.GetStartActivityIntent(this);
            StartActivity(intent);
            Finish();
        }
    }
}