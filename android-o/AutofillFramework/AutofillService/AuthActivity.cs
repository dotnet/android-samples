using Android.App;
using Android.App.Assist;
using Android.Content;
using Android.OS;
using Android.Service.Autofill;
using Android.Util;
using Android.Views.Autofill;
using Android.Widget;
using AndroidX.AppCompat.App;
using AutofillService;
using AutofillService.Datasource;

namespace AutofillFramework
{
	/**
	 * This Activity controls the UI for logging in to the Autofill service.
	 * It is launched when an Autofill Response or specific Dataset within the Response requires
	 * authentication to access. It bundles the result in an Intent.
	 */
	[Activity(Label = "AuthActivity")]
	public class AuthActivity : AppCompatActivity
	{
		// Unique id for dataset intents.
		private static int sDatasetPendingIntentId = 0;

		private EditText mMasterPassword;
		private Intent mReplyIntent;

		public static IntentSender GetAuthIntentSenderForResponse(Context context)
		{
			var intent = new Intent(context, typeof(AuthActivity));
			return PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.CancelCurrent).IntentSender;
		}

		public static IntentSender GetAuthIntentSenderForDataset(Context context, string datasetName)
		{
			var intent = new Intent(context, typeof(AuthActivity));
			intent.PutExtra(CommonUtil.EXTRA_DATASET_NAME, datasetName);
			intent.PutExtra(CommonUtil.EXTRA_FOR_RESPONSE, false);
			return PendingIntent.GetActivity(context, ++sDatasetPendingIntentId, intent,
				PendingIntentFlags.CancelCurrent).IntentSender;
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.multidataset_service_auth_activity);
			mMasterPassword = FindViewById<EditText>(Resource.Id.master_password);
			FindViewById(Resource.Id.login).Click += delegate { Login(); };

			FindViewById(Resource.Id.cancel).Click += delegate
			{
				OnFailure();
				Finish();
			};
		}

		private void Login()
		{
			var password = mMasterPassword.Text;
			if (password == MyPreferences.GetInstance(this).GetMasterPassword())
			{
				OnSuccess();
			}
			else
			{
				Toast.MakeText(this, "Password incorrect", ToastLength.Short).Show();
				OnFailure();
			}

			Finish();
		}

		public override void Finish()
		{
			if (mReplyIntent != null)
			{
				SetResult(Result.Ok, mReplyIntent);
			}
			else
			{
				SetResult(Result.Canceled);
			}

			base.Finish();
		}

		private void OnFailure()
		{
			Log.Warn(CommonUtil.TAG, "Failed auth.");
			mReplyIntent = null;
		}

		private void OnSuccess()
		{
			var intent = Intent;
			var forResponse = intent.GetBooleanExtra(CommonUtil.EXTRA_FOR_RESPONSE, true);
			var structure = (AssistStructure) intent.GetParcelableExtra(AutofillManager.ExtraAssistStructure);
			var parser = new StructureParser(ApplicationContext, structure);
			parser.ParseForFill();
			var autofillFields = parser.GetAutofillFields();
			mReplyIntent = new Intent();
			var clientFormDataMap = SharedPrefsAutofillRepository.GetInstance().GetFilledAutofillFieldCollection
				(this, autofillFields.GetFocusedHints(), autofillFields.GetAllHints());
			if (forResponse)
			{
				SetResponseIntent(AutofillHelper.NewResponse
					(this, false, autofillFields, clientFormDataMap));
			}
			else
			{
				var datasetName = intent.GetStringExtra(CommonUtil.EXTRA_DATASET_NAME);
				SetDatasetIntent(AutofillHelper.NewDataset
					(this, autofillFields, clientFormDataMap[datasetName], false));
			}
		}

		private void SetResponseIntent(FillResponse fillResponse)
		{
			mReplyIntent.PutExtra(AutofillManager.ExtraAuthenticationResult, fillResponse);
		}

		private void SetDatasetIntent(Dataset dataset)
		{
			mReplyIntent.PutExtra(AutofillManager.ExtraAuthenticationResult, dataset);
		}
	}
}