using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Autofill;
using Android.Widget;
using AndroidX.AppCompat.App;
using AutofillService.datasource;
using AutofillService.Datasource;
using AutofillService.Model;
using Google.Android.Material.Snackbar;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;

namespace AutofillService
{
    [Activity(Label = "SettingsActivity", Exported = true, MainLauncher = true)]
    [Register("com.xamarin.AutofillFramework.multidatasetservice.settings.SettingsActivity")]
    public class SettingsActivity : AppCompatActivity
    {
        private static string TAG = "SettingsActivity";
        private const int REQUEST_CODE_SET_DEFAULT = 1;
        private AutofillManager mAutofillManager;
        private static IPackageVerificationDataSource mPackageVerificationDataSource;
        private static MyPreferences preferences;
        private string mPackageName;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.multidataset_service_settings_activity);
            preferences = MyPreferences.GetInstance(this);
            SetupSettingsSwitch(
                Resource.Id.settings_auth_responses_container,
                Resource.Id.settings_auth_responses_label,
                Resource.Id.settings_auth_responses_switch,
                preferences.IsResponseAuth(),
                new CompoundButtonListener1 {that = this});
            SetupSettingsSwitch(
                Resource.Id.settings_auth_datasets_container,
                Resource.Id.settings_auth_datasets_label,
                Resource.Id.settings_auth_datasets_switch,
                preferences.IsDatasetAuth(),
                new CompoundButtonListener2 {that = this});
            SetupSettingsButton(
                Resource.Id.settings_add_data_container,
                Resource.Id.settings_add_data_label,
                Resource.Id.settings_add_data_icon,
                new ViewIOnClickListener1 {that = this});
            SetupSettingsButton(
                Resource.Id.settings_clear_data_container,
                Resource.Id.settings_clear_data_label,
                Resource.Id.settings_clear_data_icon,
                new ViewIOnClickListener2 {that = this});
            SetupSettingsButton(
                Resource.Id.settings_auth_credentials_container,
                Resource.Id.settings_auth_credentials_label,
                Resource.Id.settings_auth_credentials_icon,
                new ViewIOnClickListener3 {that = this});
        }

        public class CompoundButtonListener1 : Java.Lang.Object, CompoundButton.IOnCheckedChangeListener
        {
            public SettingsActivity that;

            public void OnCheckedChanged(CompoundButton buttonView, bool isResponseAuth)
            {
                MyPreferences.GetInstance(that).SetResponseAuth(isResponseAuth);
            }
        }

        public class CompoundButtonListener2 : Java.Lang.Object, CompoundButton.IOnCheckedChangeListener
        {
            public SettingsActivity that;

            public void OnCheckedChanged(CompoundButton buttonView, bool isDatasetAuth)
            {
                MyPreferences.GetInstance(that).SetDatasetAuth(isDatasetAuth);
            }
        }

        public class ViewIOnClickListener1 : Java.Lang.Object, View.IOnClickListener
        {
            public SettingsActivity that;

            public void OnClick(View v)
            {
                that.BuildAddDataDialog().Show();
            }
        }

        public class ViewIOnClickListener2 : Java.Lang.Object, View.IOnClickListener
        {
            public SettingsActivity that;

            public void OnClick(View v)
            {
                that.BuildClearDataDialog().Show();
            }
        }

        public class ViewIOnClickListener3 : Java.Lang.Object, View.IOnClickListener
        {
            public SettingsActivity that;

            public void OnClick(View v)
            {
                if (MyPreferences.GetInstance(that).GetMasterPassword() != null)
                {
                    that.BuildCurrentCredentialsDialog().Show();
                }
                else
                {
                    that.BuildNewCredentialsDialog().Show();
                }
            }
        }

        private AlertDialog BuildClearDataDialog()
        {
            return new AlertDialog.Builder(this)
                .SetMessage(Resource.String.settings_clear_data_confirmation)
                .SetTitle(Resource.String.settings_clear_data_confirmation_title)
                .SetNegativeButton(Resource.String.settings_cancel, new ClickListenerStub())
                .SetPositiveButton(Resource.String.settings_ok, (new PositiveButtonClickListener1 {that = this}))
                .Create();
        }

        public class ClickListenerStub : Java.Lang.Object, IDialogInterfaceOnClickListener
        {
            public void OnClick(IDialogInterface dialog, int which)
            {
            }
        }

        public class PositiveButtonClickListener1 : Java.Lang.Object, IDialogInterfaceOnClickListener
        {
            public SettingsActivity that;

            public void OnClick(IDialogInterface dialog, int which)
            {
                SharedPrefsAutofillRepository.GetInstance().Clear(that);
                SharedPrefsPackageVerificationRepository.GetInstance().Clear(that);
                MyPreferences.GetInstance(that).ClearCredentials();
                dialog.Dismiss();
            }
        }

        private AlertDialog BuildAddDataDialog()
        {
            var numberOfDatasetsPicker = LayoutInflater
                .From(this)
                .Inflate(Resource.Layout.multidataset_service_settings_add_data_dialog, null)
                .FindViewById(Resource.Id.number_of_datasets_picker) as NumberPicker;
            numberOfDatasetsPicker.MinValue = 0;
            numberOfDatasetsPicker.MaxValue = 10;
            numberOfDatasetsPicker.WrapSelectorWheel = false;
            return new AlertDialog.Builder(this)
                .SetTitle(Resource.String.settings_add_data_title)
                .SetNegativeButton(Resource.String.settings_cancel, new ClickListenerStub())
                .SetMessage(Resource.String.settings_select_number_of_datasets)
                .SetView(numberOfDatasetsPicker)
                .SetPositiveButton(Resource.String.settings_ok, new PositiveButtonClickListener2
                {
                    that = this,
                    numberOfDatasetsPicker = numberOfDatasetsPicker
                })
                .Create();
        }

        public class PositiveButtonClickListener2 : Java.Lang.Object, IDialogInterfaceOnClickListener
        {
            public SettingsActivity that;
            public NumberPicker numberOfDatasetsPicker;

            public void OnClick(IDialogInterface dialog, int which)
            {
                int numOfDatasets = numberOfDatasetsPicker.Value;
                bool success = that.BuildAndSaveMockedAutofillFieldCollection(
                    that, numOfDatasets);
                dialog.Dismiss();
                if (success)
                {
                    Snackbar.Make(that.FindViewById(Resource.Id.settings_layout),
                        that.Resources.GetQuantityString(
                            Resource.Plurals.settings_add_data_success, numOfDatasets,
                            numOfDatasets),
                        Snackbar.LengthShort).Show();
                }
            }
        }

        /**
         * Builds mock autofill data and saves it to repository.
         */
        private bool BuildAndSaveMockedAutofillFieldCollection(Context context, int numOfDatasets)
        {
            if (numOfDatasets < 0 || numOfDatasets > 10)
            {
                Log.Warn(TAG, "Number of Datasets out of range.");
                return false;
            }

            for (int i = 0; i < numOfDatasets * 2; i += 2)
            {
                foreach (int partition in AutofillHints.PARTITIONS)
                {
                    FilledAutofillFieldCollection filledAutofillFieldCollection =
                        AutofillHints.GetFakeFieldCollection(partition, i);
                    SharedPrefsAutofillRepository.GetInstance().SaveFilledAutofillFieldCollection(
                        context, filledAutofillFieldCollection);
                }
            }

            return true;
        }


        private AlertDialog.Builder PrepareCredentialsDialog()
        {
            return new AlertDialog.Builder(this)
                .SetTitle(Resource.String.settings_auth_change_credentials_title)
                .SetNegativeButton(Resource.String.settings_cancel, new ClickListenerStub());
        }

        private AlertDialog BuildCurrentCredentialsDialog()
        {
            var currentPasswordField = LayoutInflater
                .From(this)
                .Inflate(Resource.Layout.multidataset_service_settings_authentication_dialog, null)
                .FindViewById(Resource.Id.master_password_field) as EditText;
            return PrepareCredentialsDialog()
                .SetMessage(Resource.String.settings_auth_enter_current_password)
                .SetView(currentPasswordField)
                .SetPositiveButton(Resource.String.settings_ok, new DialogClickListener1
                {
                    that = this,
                    currentPasswordField = currentPasswordField
                })
                .Create();
        }

        public class DialogClickListener1 : Java.Lang.Object, IDialogInterfaceOnClickListener
        {
            public SettingsActivity that;
            public EditText currentPasswordField;

            public void OnClick(IDialogInterface dialog, int which)
            {
                string password = currentPasswordField.Text;
                if (preferences.GetMasterPassword() == password)
                {
                    that.BuildNewCredentialsDialog().Show();
                    dialog.Dismiss();
                }
            }
        }

        private AlertDialog BuildNewCredentialsDialog()
        {
            var newPasswordField = LayoutInflater
                .From(this)
                .Inflate(Resource.Layout.multidataset_service_settings_authentication_dialog, null)
                .FindViewById(Resource.Id.master_password_field) as EditText;
            return PrepareCredentialsDialog()
                .SetMessage(Resource.String.settings_auth_enter_new_password)
                .SetView(newPasswordField)
                .SetPositiveButton(Resource.String.settings_ok, new DialogClickListener2
                {
                    that = this,
                    newPasswordField = newPasswordField
                })
                .Create();
        }

        public class DialogClickListener2 : Java.Lang.Object, IDialogInterfaceOnClickListener
        {
            public SettingsActivity that;
            public EditText newPasswordField;

            public void OnClick(IDialogInterface dialog, int which)
            {
                string password = newPasswordField.Text;
                preferences.SetMasterPassword(password);
                dialog.Dismiss();
            }
        }

        private void SetupSettingsSwitch(
            int containerId,
            int labelId,
            int switchId,
            bool isChecked,
            CompoundButton.IOnCheckedChangeListener checkedChangeListener
        )
        {
            var container = FindViewById<ViewGroup>(containerId);
            string switchLabel = container.FindViewById<TextView>(labelId).Text;
            var switchView = container.FindViewById<Switch>(switchId);
            switchView.ContentDescription = switchLabel;
            switchView.Checked = isChecked;
            container.Click += delegate { switchView.PerformClick(); };
            switchView.SetOnCheckedChangeListener(checkedChangeListener);
        }

        private void SetupSettingsButton(
            int containerId,
            int labelId,
            int imageViewId,
            View.IOnClickListener onClickListener
        )
        {
            var container = FindViewById<ViewGroup>(containerId);
            var buttonLabel = container.FindViewById<TextView>(labelId);
            var buttonLabelText = buttonLabel.Text;
            var imageView = container.FindViewById<ImageView>(imageViewId);
            imageView.ContentDescription = buttonLabelText;
            container.SetOnClickListener(onClickListener);
        }
    }
}