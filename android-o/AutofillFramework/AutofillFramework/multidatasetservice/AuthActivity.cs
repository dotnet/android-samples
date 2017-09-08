using System;
using Android.App;
using Android.App.Assist;
using Android.Content;
using Android.OS;
using Android.Service.Autofill;
using Android.Support.V7.App;
using Android.Util;
using Android.Views.Autofill;
using Android.Widget;
using AutofillFramework.multidatasetservice.datasource;
using AutofillFramework.multidatasetservice.settings;

namespace AutofillFramework
{
	/// <summary>
	/// This Activity controls the UI for logging in to the Autofill service.
	/// It is launched when an Autofill Response or specific Dataset within the Response requires
	/// authentication to access. It bundles the result in an Intent.
	/// </summary>
	[Activity(Label = "AuthActivity")]
	public class AuthActivity : AppCompatActivity
	{
		/// <summary>
		/// Unique id for dataset intents.
		/// </summary>
		static int DatasetPendingIntentId = 0;

		EditText MasterPassword;
		Intent ReplyIntent;

		public static IntentSender GetAuthIntentSenderForResponse(Context context)
		{
			var intent = new Intent(context, typeof(AuthActivity));
			return PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.CancelCurrent).IntentSender;
		}

		public static IntentSender GetAuthIntentSenderForDataset(Context context, String datasetName)
		{
			var intent = new Intent(context, typeof(AuthActivity));
	        intent.PutExtra(CommonUtil.EXTRA_DATASET_NAME, datasetName);
	        intent.PutExtra(CommonUtil.EXTRA_FOR_RESPONSE, false);
	        return PendingIntent.GetActivity(context, ++DatasetPendingIntentId, intent,
			                                 PendingIntentFlags.CancelCurrent).IntentSender;
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.multidataset_service_auth_activity);
			MasterPassword = (EditText)FindViewById(Resource.Id.master_password);
			FindViewById(Resource.Id.login).Click += (sender, e) => {
				Login();
			};
			FindViewById(Resource.Id.cancel).Click += (sender, e) => {
				OnFailure();
				Finish();
			};
		}

		void Login()
		{
			var password = MasterPassword.Text;
			if (password.Equals(MyPreferences.GetInstance(this).GetMasterPassword()))
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
			if (ReplyIntent != null)
			{
				SetResult(Result.Ok, ReplyIntent);
			}
			else
			{
				SetResult(Result.Canceled);
			}
			base.Finish();
		}

	 	void OnFailure()
		{
			Log.Warn(CommonUtil.Tag, "Failed auth.");
			ReplyIntent = null;
		}

		protected void OnSuccess()
		{
			var intent = Intent;
			var forResponse = intent.GetBooleanExtra(CommonUtil.EXTRA_FOR_RESPONSE, true);
			AssistStructure structure = (AssistStructure)intent.GetParcelableExtra(AutofillManager.ExtraAssistStructure);
			StructureParser parser = new StructureParser(structure);
			parser.ParseForFill();
			AutofillFieldMetadataCollection autofillFields = parser.AutofillFields;
			var saveTypes = autofillFields.SaveType;
			ReplyIntent = new Intent();
			var clientFormDataMap = SharedPrefsAutofillRepository.GetInstance(this).GetFilledAutofillFieldCollection
			                                                     (autofillFields.FocusedAutofillHints, autofillFields.AllAutofillHints);
			if (forResponse)
			{
				SetResponseIntent(AutofillHelper.NewResponse(this, false, autofillFields, clientFormDataMap));
			}
			else
			{
				String datasetName = intent.GetStringExtra(CommonUtil.EXTRA_DATASET_NAME);
				SetDatasetIntent(AutofillHelper.NewDataset(this, autofillFields, clientFormDataMap[datasetName], false));
			}
		}

		void SetResponseIntent(FillResponse fillResponse)
		{
			ReplyIntent.PutExtra(AutofillManager.ExtraAuthenticationResult, fillResponse);
		}

		void SetDatasetIntent(Dataset dataset)
		{
			ReplyIntent.PutExtra(AutofillManager.ExtraAuthenticationResult, dataset);
		}
	}
}
