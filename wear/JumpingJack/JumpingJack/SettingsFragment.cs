
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace JumpingJack
{
	public class SettingsFragment : Android.Support.V4.App.Fragment
	{
		private Button button;
		private MainActivity main_activity;

		public SettingsFragment(MainActivity mainActivity)
		{
			main_activity = mainActivity;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View view = inflater.Inflate (Resource.Layout.setting_layout, container, false);
			button = (Button) view.FindViewById(Resource.Id.btn);
			button.Click+= delegate {
				main_activity.ResetCounter();
			};
			return view;
		}
	}
}

