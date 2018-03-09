using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.Autofill;
using Android.Widget;
using Java.Lang;

namespace AutofillFramework
{
	[Activity(Label = "MultiplePartitionsActivity")]
	[Register("com.xamarin.AutofillFramework.MultiplePartitionsActivity")]
	public class MultiplePartitionsActivity : AppCompatActivity
	{
		private ScrollableCustomVirtualView mCustomVirtualView;
		private AutofillManager mAutofillManager;

		private CustomVirtualView.Partition mCredentialsPartition;
		private CustomVirtualView.Partition mCcPartition;

		public static Intent GetStartActivityIntent(Context context)
		{
			var intent = new Intent(context, typeof(MultiplePartitionsActivity));
			return intent;
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.multiple_partitions_activity);

			mCustomVirtualView = FindViewById<ScrollableCustomVirtualView>(Resource.Id.custom_view);


			mCredentialsPartition =
				mCustomVirtualView.AddPartition(GetString(Resource.String.partition_credentials));
			mCredentialsPartition.AddLine("username", AutofillType.Text,
				GetString(Resource.String.username_label),
				"         ", false, View.AutofillHintUsername);
			mCredentialsPartition.AddLine("password", AutofillType.Text,
				GetString(Resource.String.password_label),
				"         ", true, View.AutofillHintPassword);

			AutofillType ccExpirationType = AutofillType.Date;
			// TODO: add a checkbox to switch between text / date instead
			Intent intent = Intent;
			if (intent != null)
			{
				var newType = intent.GetIntExtra("dateType", -1);
				if (newType != -1)
				{
					ccExpirationType = (AutofillType) newType;
					var typeMessage = GetString(Resource.String.message_credit_card_expiration_type,
						CommonUtil.GetTypeAsString(ccExpirationType));
					// TODO: display type in a header or proper status widget
					Toast.MakeText(ApplicationContext, typeMessage, ToastLength.Long).Show();
				}
			}

			mCcPartition = mCustomVirtualView.AddPartition(GetString(Resource.String.partition_credit_card));
			mCcPartition.AddLine("ccNumber", AutofillType.Text,
				GetString(Resource.String.credit_card_number_label),
				"         ", true, View.AutofillHintCreditCardNumber);
			mCcPartition.AddLine("ccDay", AutofillType.Text,
				GetString(Resource.String.credit_card_expiration_day_label),
				"         ", true, View.AutofillHintCreditCardExpirationDay);
			mCcPartition.AddLine("ccMonth", ccExpirationType,
				GetString(Resource.String.credit_card_expiration_month_label),
				"         ", true, View.AutofillHintCreditCardExpirationMonth);
			mCcPartition.AddLine("ccYear", AutofillType.Text,
				GetString(Resource.String.credit_card_expiration_year_label),
				"         ", true, View.AutofillHintCreditCardExpirationYear);
			mCcPartition.AddLine("ccDate", ccExpirationType,
				GetString(Resource.String.credit_card_expiration_date_label),
				"         ", true, View.AutofillHintCreditCardExpirationDate);
			mCcPartition.AddLine("ccSecurityCode", AutofillType.Text,
				GetString(Resource.String.credit_card_security_code_label),
				"         ", true, View.AutofillHintCreditCardSecurityCode);

			FindViewById(Resource.Id.clear).Click += delegate
			{
				ResetFields();
				mCustomVirtualView.ResetPositions();
				mAutofillManager.Cancel();
			};

			mAutofillManager = (AutofillManager) GetSystemService(Class.FromType(typeof(AutofillManager)));
		}

		private void ResetFields()
		{
			mCredentialsPartition.Reset();
			mCcPartition.Reset();
			mCustomVirtualView.PostInvalidate();
		}
	}
}