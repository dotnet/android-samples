using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Java.Lang;
using Java.Util;

namespace AutofillFramework.app
{
	[Activity(Label = "CreditCardActivity")]
	public class CreditCardActivity : AppCompatActivity
	{
		static int CC_EXP_YEARS_COUNT = 5;

		string[] Years = new string[CC_EXP_YEARS_COUNT];

    	Spinner mCcExpirationDaySpinner;
		Spinner mCcExpirationMonthSpinner;
		Spinner mCcExpirationYearSpinner;
		EditText mCcCardNumber;

		public static Intent GetStartActivityIntent(Context context)
		{
			var intent = new Intent(context, typeof(CreditCardActivity));
        	return intent;
    	}

		class YearAdapter : ArrayAdapter
		{
			string[] Years { get; set; }
			public YearAdapter(Context context, string[] years) : base(context, Android.Resource.Layout.SimpleSpinnerItem, years)
			{
				Years = years;
			}

			public override ICharSequence[] GetAutofillOptionsFormatted()
			{
				// convert C# string into Java String to return a CharSequence array
				var javaStringList = new String[Years.Count()];
				for (var pos = 0; pos < Years.Count(); pos++) {
					javaStringList[pos] = new String(Years[pos]);
				}
				return javaStringList;
			}
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.credit_card_activity);
			mCcExpirationDaySpinner = (Spinner)FindViewById(Resource.Id.expirationDay);
			mCcExpirationMonthSpinner = (Spinner)FindViewById(Resource.Id.expirationMonth);
			mCcExpirationYearSpinner = (Spinner)FindViewById(Resource.Id.expirationYear);
			mCcCardNumber = (EditText)FindViewById(Resource.Id.creditCardNumberField);

			// Create an ArrayAdapter using the string array and a default spinner layout
			ArrayAdapter dayAdapter = ArrayAdapter.CreateFromResource
			           (this, Resource.Array.day_array, Android.Resource.Layout.SimpleSpinnerItem);
			// Specify the layout to use when the list of choices appears
			dayAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerItem);
			// Apply the adapter to the spinner
			mCcExpirationDaySpinner.Adapter = dayAdapter;

			ArrayAdapter monthAdapter = ArrayAdapter.CreateFromResource
			           (this, Resource.Array.month_array, Android.Resource.Layout.SimpleSpinnerItem);
			monthAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerItem);
			mCcExpirationMonthSpinner.Adapter = monthAdapter;
			int year = Calendar.GetInstance(Locale.Default).Get(CalendarField.Year);
			for (int i = 0; i < Years.Length; i++)
			{
				Years[i] = Integer.ToString(year + i);
			}

			mCcExpirationYearSpinner.Adapter = new YearAdapter(this, Years);
			FindViewById(Resource.Id.submit).Click += (sender, args) => {
				Submit();
			};
			FindViewById(Resource.Id.clear).Click += (sender, args) => {
				ResetFields();
			};
		}

		void ResetFields()
		{
			mCcExpirationDaySpinner.SetSelection(0);
			mCcExpirationMonthSpinner.SetSelection(0);
			mCcExpirationYearSpinner.SetSelection(0);
			mCcCardNumber.Text = "";
		}

		/// <summary>
		/// Launches new Activity and finishes, triggering an autofill save request if the user entered
		/// any new data.
		/// </summary>
		void Submit()
		{
			Intent intent = WelcomeActivity.GetStartActivityIntent(this);
			StartActivity(intent);
			Finish();
		}
	}
}
