using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using AutofillFramework.app;
using static Android.Views.View;
using Android.Views;
using Android.Runtime;

namespace AutofillFramework
{
	/// <summary>
	/// This is used to launch sample activities that showcase autofill.
	/// </summary>
	[Activity(Label = "Autofill Framework", MainLauncher = true)]
	[Register("com.xamarin.AutofillFramework.app.MainActivity")]
	public class MainActivity : AppCompatActivity
	{
		abstract class BaseListenerImpl : Java.Lang.Object
		{
			public MainActivity Context { get; set; }
		}

		class LoginEditTextsListenerImpl : BaseListenerImpl, IOnClickListener
		{
			public void OnClick(View v)
			{
				Context.StartActivity(StandardSignInActivity.GetStartActivityIntent(Context));
			}
		}

		class LoginCustomVirtualListenerImpl : BaseListenerImpl, IOnClickListener
		{
			public void OnClick(View v)
			{
				Context.StartActivity(VirtualSignInActivity.GetStartActivityIntent(Context));
			}
		}

		class CreditCardSpinnerListenerImpl : BaseListenerImpl, IOnClickListener
		{
			public void OnClick(View v)
			{
				Context.StartActivity(CreditCardActivity.GetStartActivityIntent(Context));
			}
		}

 		class LoginAutoCompleteListenerImpl : BaseListenerImpl, IOnClickListener
		{
			public void OnClick(View v)
			{
				Context.StartActivity(StandardAutoCompleteSignInActivity.GetStartActivityIntent(Context));
			}
		}

		class EmailComposeListenerImpl : BaseListenerImpl, IOnClickListener
		{
			public void OnClick(View v)
			{
				Context.StartActivity(EmailComposeActivity.GetStartActivityIntent(Context));
			}
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{	
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.activity_main);

			NavigationItem loginEditTexts = (NavigationItem)FindViewById(Resource.Id.standardViewSignInButton);
			NavigationItem loginCustomVirtual = (NavigationItem)FindViewById(Resource.Id.virtualViewSignInButton);
			NavigationItem creditCardSpinners = (NavigationItem)FindViewById(Resource.Id.creditCardCheckoutButton);
			NavigationItem loginAutoComplete = (NavigationItem)FindViewById(Resource.Id.standardLoginWithAutoCompleteButton);
			NavigationItem emailCompose = (NavigationItem)FindViewById(Resource.Id.emailComposeButton);

			loginEditTexts.SetNavigationButtonClickListener(new LoginEditTextsListenerImpl {Context = this});
			loginCustomVirtual.SetNavigationButtonClickListener(new LoginCustomVirtualListenerImpl { Context = this });
			creditCardSpinners.SetNavigationButtonClickListener(new CreditCardSpinnerListenerImpl { Context = this });
			loginAutoComplete.SetNavigationButtonClickListener(new LoginAutoCompleteListenerImpl { Context = this });
			emailCompose.SetNavigationButtonClickListener(new EmailComposeListenerImpl { Context = this });
		}
	}
}

