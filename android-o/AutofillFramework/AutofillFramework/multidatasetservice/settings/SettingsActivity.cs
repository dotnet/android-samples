using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AutofillFramework.multidatasetservice.datasource;

namespace AutofillFramework.multidatasetservice.settings
{
	[Activity(Label = "SettingsActivity", Exported = true, MainLauncher = true)]
	[Register("com.xamarin.AutofillFramework.multidatasetservice.settings.SettingsActivity")]
	public class SettingsActivity : AppCompatActivity
	{
		Android.Support.V7.App.AlertDialog ClearDataDialog;
		Android.Support.V7.App.AlertDialog CurrentCredentialsDialog;
		Android.Support.V7.App.AlertDialog NewCredentialsDialog;

		class OnAuthResponseCheckedChange : Java.Lang.Object, CompoundButton.IOnCheckedChangeListener
		{
			public MyPreferences preferences { get; set; }
			public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
			{
				preferences.SetResponseAuth(isChecked);
			}
		}

		class OnAuthDatasetCheckedChange : Java.Lang.Object, CompoundButton.IOnCheckedChangeListener
		{
			public MyPreferences preferences { get; set; }
			public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
			{
				preferences.SetDatasetAuth(isChecked);
			}
		}

		class ClearDataButtonClick : Java.Lang.Object, View.IOnClickListener
		{
			public SettingsActivity Activity { get; set; }
			public void OnClick(View v)
			{
				Activity.BuildClearDataDialog().Show();
			}
		}

		class AuthCredendialsButtonClick : Java.Lang.Object, View.IOnClickListener
		{
			public SettingsActivity Activity { get; set; }

			public void OnClick(View v)
			{
				if (MyPreferences.GetInstance(Activity).GetMasterPassword() != null)
				{
					Activity.BuildCurrentCredentialsDialog().Show();
				}
				else
				{
					Activity.BuildNewCredentialsDialog().Show();
				}
			}
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.multidataset_service_settings_activity);

			var preferences = MyPreferences.GetInstance(this);
			SetupSettingsSwitch(Resource.Id.settings_auth_responses_container,
				Resource.Id.settings_auth_responses_label,
				Resource.Id.settings_auth_responses_switch,
			    preferences.IsResponseAuth(), 
			    new OnAuthResponseCheckedChange { preferences = preferences });

			SetupSettingsSwitch(Resource.Id.settings_auth_datasets_container,
				Resource.Id.settings_auth_datasets_label,
				Resource.Id.settings_auth_datasets_switch,
			    preferences.IsDatasetAuth(),
				new OnAuthDatasetCheckedChange { preferences = preferences });

			SetupSettingsButton(Resource.Id.settings_clear_data_container,
				Resource.Id.settings_clear_data_label,
			    Resource.Id.settings_clear_data_icon, new ClearDataButtonClick { Activity = this });
			
			SetupSettingsButton(Resource.Id.settings_auth_credentials_container,
				Resource.Id.settings_auth_credentials_label,
			    Resource.Id.settings_auth_credentials_icon, new AuthCredendialsButtonClick { Activity = this });
		}

		Android.Support.V7.App.AlertDialog BuildClearDataDialog()
		{
			 var builder = new Android.Support.V7.App.AlertDialog.Builder(this)
				 .SetMessage(Resource.String.settings_clear_data_confirmation)
				 .SetTitle(Resource.String.settings_clear_data_confirmation_title)
			     .SetNegativeButton(Resource.String.cancel, (sender, args) => {
						 // Do nothing...
					 })
				 .SetPositiveButton(Resource.String.ok, (sender, args) => 
					 {
						 SharedPrefsAutofillRepository.GetInstance(this).Clear();
						 MyPreferences.GetInstance(this).ClearCredentials();
						 DismissDataDialog();
					 });
			ClearDataDialog = builder.Create();
			return ClearDataDialog;
		}

		Android.Support.V7.App.AlertDialog.Builder PrepareCredentialsDialog()
		{
			return new Android.Support.V7.App.AlertDialog.Builder(this)
				 .SetTitle(Resource.String.settings_auth_change_credentials_title)
				 .SetNegativeButton(Resource.String.cancel, (sender, args) => {
						 // Do nothing...
					 });
		}

		Android.Support.V7.App.AlertDialog BuildCurrentCredentialsDialog()
		{
			EditText currentPasswordField = (EditText) LayoutInflater.From(this)
				   .Inflate(Resource.Layout.multidataset_service_settings_authentication_dialog, null)
				   .FindViewById(Resource.Id.master_password_field);
			CurrentCredentialsDialog = PrepareCredentialsDialog()
				.SetMessage(Resource.String.settings_auth_enter_current_password)
				.SetView(currentPasswordField)
				.SetPositiveButton(Resource.String.ok, (sender, e) =>
					{
						var password = currentPasswordField.Text;
						if (MyPreferences.GetInstance(this).GetMasterPassword().Equals(password))
						{
							BuildNewCredentialsDialog().Show();
							DismissCurrentCredentialsDialog();
						}
					})
				.Create();
			return CurrentCredentialsDialog;
		}

		Android.Support.V7.App.AlertDialog BuildNewCredentialsDialog()
		{
			EditText newPasswordField = (EditText) LayoutInflater.From(this)
				   .Inflate(Resource.Layout.multidataset_service_settings_authentication_dialog, null)
				   .FindViewById(Resource.Id.master_password_field);
			NewCredentialsDialog = PrepareCredentialsDialog()
				.SetMessage(Resource.String.settings_auth_enter_new_password)
				.SetView(newPasswordField)
				.SetPositiveButton(Resource.String.ok, (sender, e) =>
				{
					var password = newPasswordField.Text;
					MyPreferences.GetInstance(this).SetMasterPassword(password);
					DismissNewCredentialsDialog();
				})
				.Create();
			return NewCredentialsDialog;
		}

		void SetupSettingsSwitch(int containerId, int labelId, int switchId, bool IsChecked,
			CompoundButton.IOnCheckedChangeListener checkedChangeListener)
		{
			ViewGroup container = (ViewGroup)FindViewById(containerId);
			var switchLabel = ((TextView)container.FindViewById(labelId)).Text;
			Switch switchView = (Switch)container.FindViewById(switchId);
			switchView.ContentDescription = switchLabel;
			switchView.Checked = IsChecked;
			container.Click += (sender, e) => {
				switchView.PerformClick();
			};
			switchView.SetOnCheckedChangeListener(checkedChangeListener);
		}

		void SetupSettingsButton(int containerId, int labelId, int imageViewId, View.IOnClickListener onClickListener)
		{
			ViewGroup container = (ViewGroup)FindViewById(containerId);
			var buttonLabel = ((TextView)container.FindViewById(labelId)).Text;
			ImageView imageView = (ImageView)container.FindViewById(imageViewId);
			imageView.ContentDescription = buttonLabel;
			container.SetOnClickListener(onClickListener);
		}

		void DismissDataDialog()
		{
			if (ClearDataDialog != null) ClearDataDialog.Dismiss();
		}

		void DismissCurrentCredentialsDialog()
		{
			if (CurrentCredentialsDialog != null) CurrentCredentialsDialog.Dismiss();
		}

		void DismissNewCredentialsDialog()
		{
			if (NewCredentialsDialog != null) NewCredentialsDialog.Dismiss();
		}

	}
}
