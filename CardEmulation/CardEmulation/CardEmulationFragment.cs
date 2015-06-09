
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Text;
using Android.App;

namespace CardEmulation
{
	public class CardEmulationFragment : Fragment
	{
		public const string TAG = "CardEmulationFragment";
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			//Inflates the layout for this fragment
			View v = inflater.Inflate (Resource.Layout.main_fragment, container, false);
			EditText account = (EditText)v.FindViewById (Resource.Id.card_account_field);
			account.SetText(AccountStorage.GetAccount(Activity), TextView.BufferType.Editable);
			account.AddTextChangedListener (new AccountUpdater () { Activity = this.Activity });
			return v;
		}

		private class AccountUpdater : Java.Lang.Object, ITextWatcher
		{
			public Activity Activity;
			#region ITextWatcher implementation

			public void AfterTextChanged (IEditable s)
			{
				String account = s.ToString ();
				AccountStorage.SetAccount (Activity, account);
			}

			public void BeforeTextChanged (Java.Lang.ICharSequence s, int start, int count, int after)
			{
				//Not implemented
			}

			public void OnTextChanged (Java.Lang.ICharSequence s, int start, int before, int count)
			{
				//Not implemented
			}

			#endregion


		}
	}
}

