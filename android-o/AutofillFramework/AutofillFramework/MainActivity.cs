using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using AndroidX.AppCompat.App;
using Java.Lang;

namespace AutofillFramework
{
    [Activity(Label = "MainActivity", MainLauncher = true)]
    [Register("com.xamarin.AutofillFramework.MainActivity")]
    public class MainActivity : AppCompatActivity
    {
        private static string TAG = "MainActivity";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (LaunchTrampolineActivity())
            {
                return;
            }

            SetContentView(Resource.Layout.activity_main);
            var loginEditTexts = FindViewById<NavigationItem>(Resource.Id.standardViewSignInButton);
            var loginCustomVirtual = FindViewById<NavigationItem>(Resource.Id.virtualViewSignInButton);
            var creditCard = FindViewById<NavigationItem>(Resource.Id.creditCardButton);
            var creditCardSpinners = FindViewById<NavigationItem>(Resource.Id.creditCardSpinnersButton);
            var loginAutoComplete = FindViewById<NavigationItem>(Resource.Id.standardLoginWithAutoCompleteButton);
            var emailCompose = FindViewById<NavigationItem>(Resource.Id.emailComposeButton);
            var creditCardCompoundView = FindViewById<NavigationItem>(Resource.Id.creditCardCompoundViewButton);
            var creditCardDatePicker = FindViewById<NavigationItem>(Resource.Id.creditCardDatePickerButton);
            var creditCardAntiPatternPicker = FindViewById<NavigationItem>(Resource.Id.creditCardAntiPatternButton);
            var multiplePartitions = FindViewById<NavigationItem>(Resource.Id.multiplePartitionsButton);
            var loginWebView = FindViewById<NavigationItem>(Resource.Id.webviewSignInButton);

            loginEditTexts.SetNavigationButtonClickListener(new LoginEditTextsClickListener {that = this});
            loginCustomVirtual.SetNavigationButtonClickListener(new LoginCustomVirtualClickListener {that = this});
            creditCard.SetNavigationButtonClickListener(new CreditCardClickListener {that = this});
            creditCardSpinners.SetNavigationButtonClickListener(new CreditCardSpinnersClickListener {that = this});
            loginAutoComplete.SetNavigationButtonClickListener(new LoginAutoCompleteClickListener {that = this});
            emailCompose.SetNavigationButtonClickListener(new EmailComposeClickListener {that = this});
            creditCardCompoundView.SetNavigationButtonClickListener(
                new CreditCardCompoundViewClickListener {that = this});
            creditCardDatePicker.SetNavigationButtonClickListener(new CreditCardDatePickerClickListener {that = this});
            creditCardAntiPatternPicker.SetNavigationButtonClickListener(
                new CreditCardAntiPatternPickerClickListener {that = this});
            multiplePartitions.SetNavigationButtonClickListener(new MultiplePartitionsClickListener {that = this});

            loginWebView.SetNavigationButtonClickListener(new LoginWebViewClickListener {that = this});
        }

        public class LoginEditTextsClickListener : Java.Lang.Object, View.IOnClickListener
        {
            public MainActivity that;

            public void OnClick(View v)
            {
                that.StartActivity(StandardSignInActivity.GetStartActivityIntent(that));
            }
        }

        public class LoginCustomVirtualClickListener : Java.Lang.Object, View.IOnClickListener
        {
            public MainActivity that;

            public void OnClick(View v)
            {
                that.StartActivity(VirtualSignInActivity.GetStartActivityIntent(that));
            }
        }

        public class CreditCardClickListener : Java.Lang.Object, View.IOnClickListener
        {
            public MainActivity that;

            public void OnClick(View v)
            {
                that.StartActivity(CreditCardActivity.GetStartActivityIntent(that));
            }
        }

        public class CreditCardSpinnersClickListener : Java.Lang.Object, View.IOnClickListener
        {
            public MainActivity that;

            public void OnClick(View v)
            {
                that.StartActivity(CreditCardSpinnersActivity.GetStartActivityIntent(that));
            }
        }

        public class LoginAutoCompleteClickListener : Java.Lang.Object, View.IOnClickListener
        {
            public MainActivity that;

            public void OnClick(View v)
            {
                that.StartActivity(StandardAutoCompleteSignInActivity.GetStartActivityIntent(that));
            }
        }

        public class EmailComposeClickListener : Java.Lang.Object, View.IOnClickListener
        {
            public MainActivity that;

            public void OnClick(View v)
            {
                that.StartActivity(EmailComposeActivity.GetStartActivityIntent(that));
            }
        }

        public class CreditCardCompoundViewClickListener : Java.Lang.Object, View.IOnClickListener
        {
            public MainActivity that;

            public void OnClick(View v)
            {
                that.StartActivity(CreditCardCompoundViewActivity.GetStartActivityIntent(that));
            }
        }

        public class CreditCardDatePickerClickListener : Java.Lang.Object, View.IOnClickListener
        {
            public MainActivity that;

            public void OnClick(View v)
            {
                that.StartActivity(CreditCardDatePickerActivity.GetStartActivityIntent(that));
            }
        }

        public class CreditCardAntiPatternPickerClickListener : Java.Lang.Object, View.IOnClickListener
        {
            public MainActivity that;

            public void OnClick(View v)
            {
                that.StartActivity(CreditCardAntiPatternActivity.GetStartActivityIntent(that));
            }
        }

        public class MultiplePartitionsClickListener : Java.Lang.Object, View.IOnClickListener
        {
            public MainActivity that;

            public void OnClick(View v)
            {
                that.StartActivity(MultiplePartitionsActivity.GetStartActivityIntent(that));
            }
        }

        public class LoginWebViewClickListener : Java.Lang.Object, View.IOnClickListener
        {
            public MainActivity that;

            public void OnClick(View v)
            {
                that.StartActivity(WebViewSignInActivity.GetStartActivityIntent(that));
            }
        }

        private bool LaunchTrampolineActivity()
        {
            var intent = Intent;
            if (intent != null)
            {
                var target = intent.GetStringExtra("target");
                if (target != null)
                {
                    Log.Info(TAG, "trampolining into " + target + " instead");
                    try
                    {
                        var newIntent = new Intent(this,
                            Class.ForName("AutofillFramework." + target));
                        newIntent.PutExtras(intent);
                        newIntent.RemoveExtra("target");
                        ApplicationContext.StartActivity(newIntent);
                        Finish();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Log.Error(TAG, "Error launching " + target, e);
                    }
                }
            }
            return false;
        }
    }
}