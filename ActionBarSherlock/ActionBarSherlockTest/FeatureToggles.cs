
// C# port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.

using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Xamarin.ActionbarSherlockBinding.App;
using SherlockActionBar = Xamarin.ActionbarSherlockBinding.App.ActionBar;
using Xamarin.ActionbarSherlockBinding.Views;
using Tab = Xamarin.ActionbarSherlockBinding.App.ActionBar.Tab;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

namespace Mono.ActionbarsherlockTest
{
	[Activity (Label = "@string/feature_toggles")]
	[IntentFilter (new string [] { Intent.ActionMain },
		Categories = new string [] { Constants.DemoCategory })]
	public class FeatureToggles : SherlockActivity, SherlockActionBar.ITabListener
	{
		private static readonly Random RANDOM = new Random ();
		private int items = 0;

		public override bool OnCreateOptionsMenu (Xamarin.ActionbarSherlockBinding.Views.IMenu menu)
		{
			for (int i = 0; i < items; i++) {
				menu.Add ("Text")
					.SetIcon (Resource.Drawable.ic_title_share_default)
						.SetShowAsAction (MenuItem.ShowAsActionIfRoom | MenuItem.ShowAsActionWithText);
			}

			return base.OnCreateOptionsMenu (menu);
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetTheme (SampleList.THEME); //Used for theme switching in samples
			RequestWindowFeature (WindowFeatures.Progress);
			RequestWindowFeature (WindowFeatures.IndeterminateProgress);
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.feature_toggles);
			SetSupportProgressBarIndeterminateVisibility (false);
			SetSupportProgressBarVisibility (false);

			SupportActionBar.SetCustomView (Resource.Layout.custom_view);
			SupportActionBar.SetDisplayShowCustomEnabled (false);

			Context context = SupportActionBar.ThemedContext;
			var listAdapter = ArrayAdapter.CreateFromResource (context, Resource.Array.locations, Resource.Layout.sherlock_spinner_item);
			listAdapter.SetDropDownViewResource (Resource.Layout.sherlock_spinner_dropdown_item);

			SupportActionBar.SetListNavigationCallbacks (listAdapter, null);

			FindViewById (Resource.Id.display_progress_show).Click += delegate {
				SetSupportProgressBarVisibility (true);
				SetSupportProgressBarIndeterminateVisibility (false);
				SetSupportProgress (RANDOM.Next (8000) + 10);
			};
			FindViewById (Resource.Id.display_progress_hide).Click += delegate {
				SetSupportProgressBarVisibility (false);
			};
			FindViewById (Resource.Id.display_iprogress_show).Click += delegate {
				//Hack to hide the regular progress bar
				SetSupportProgress ((int) Xamarin.ActionbarSherlockBinding.Views.Window.ProgressEnd);
				SetSupportProgressBarIndeterminateVisibility (true);
			};
			FindViewById (Resource.Id.display_iprogress_hide).Click += delegate {
				SetSupportProgressBarIndeterminateVisibility (false);
			};

			FindViewById (Resource.Id.display_items_clear).Click += delegate {
				items = 0;
				InvalidateOptionsMenu ();
			};
			FindViewById (Resource.Id.display_items_add).Click += delegate {
				items += 1;
				InvalidateOptionsMenu ();
			};

			FindViewById (Resource.Id.display_subtitle_show).Click += delegate {
				SupportActionBar.Subtitle = "The quick brown fox jumps over the lazy dog.";
			};
			FindViewById (Resource.Id.display_subtitle_hide).Click += delegate {
				SupportActionBar.Subtitle = null;
			};

			FindViewById (Resource.Id.display_title_show).Click += delegate {
				SupportActionBar.SetDisplayShowTitleEnabled (true);
			};
			FindViewById (Resource.Id.display_title_hide).Click += delegate {
				SupportActionBar.SetDisplayShowTitleEnabled (false);
			};

			FindViewById (Resource.Id.display_custom_show).Click += delegate {
				SupportActionBar.SetDisplayShowCustomEnabled (true);
			};
			FindViewById (Resource.Id.display_custom_hide).Click += delegate {
				SupportActionBar.SetDisplayShowCustomEnabled (false);
			};

			FindViewById (Resource.Id.navigation_standard).Click += delegate {
				SupportActionBar.NavigationMode = SherlockActionBar.NavigationModeStandard;
			};
			FindViewById (Resource.Id.navigation_list).Click += delegate {
				SupportActionBar.NavigationMode = SherlockActionBar.NavigationModeList;
			};
			FindViewById (Resource.Id.navigation_tabs).Click += delegate {
				SupportActionBar.NavigationMode = SherlockActionBar.NavigationModeTabs;
			};

			FindViewById (Resource.Id.display_home_as_up_show).Click += delegate {
				SupportActionBar.SetDisplayHomeAsUpEnabled (true);
			};
			FindViewById (Resource.Id.display_home_as_up_hide).Click += delegate {
				SupportActionBar.SetDisplayHomeAsUpEnabled (false);
			};

			FindViewById (Resource.Id.display_logo_show).Click += delegate {
				SupportActionBar.SetDisplayUseLogoEnabled (true);
			};
			FindViewById (Resource.Id.display_logo_hide).Click += delegate {
				SupportActionBar.SetDisplayUseLogoEnabled (false);
			};

			FindViewById (Resource.Id.display_home_show).Click += delegate {
				SupportActionBar.SetDisplayShowHomeEnabled (true);
			};
			FindViewById (Resource.Id.display_home_hide).Click += delegate {
				SupportActionBar.SetDisplayShowHomeEnabled (false);
			};

			FindViewById (Resource.Id.display_actionbar_show).Click += delegate {
				SupportActionBar.Show ();
			};
			FindViewById (Resource.Id.display_actionbar_hide).Click += delegate {
				SupportActionBar.Hide ();
			};

			Button tabAdd = FindViewById<Button> (Resource.Id.display_tab_add);
			tabAdd.Click += delegate {
				SherlockActionBar.Tab newTab = SupportActionBar.NewTab ();

				if (RANDOM.Next () % 2 == 1) {
					newTab.SetCustomView (Resource.Layout.tab_custom_view);
				} else {
					bool icon = RANDOM.Next () % 2 == 1;
					if (icon) {
						newTab.SetIcon (Resource.Drawable.ic_title_share_default);
					}
					if (!icon || RANDOM.Next () % 2 == 1) {
						newTab.SetText ("Text!");
					}
				}
				newTab.SetTabListener (this);
				SupportActionBar.AddTab (newTab);
			};
			//Add some tabs
			tabAdd.PerformClick ();
			tabAdd.PerformClick ();
			tabAdd.PerformClick ();

			FindViewById (Resource.Id.display_tab_select).Click += delegate {
				if (SupportActionBar.TabCount > 0) {
					SupportActionBar.SelectTab (
						SupportActionBar.GetTabAt (
						RANDOM.Next (SupportActionBar.TabCount)
					)
					);
				}
			};
			FindViewById (Resource.Id.display_tab_remove).Click += delegate {
				if (SupportActionBar.TabCount > 0) {
					SupportActionBar.RemoveTabAt (SupportActionBar.TabCount - 1);
				}
			};
			FindViewById (Resource.Id.display_tab_remove_all).Click += delegate {
				SupportActionBar.RemoveAllTabs ();
			};
		}

		public void OnTabSelected (Tab tab, FragmentTransaction transaction)
		{
		}

		public void OnTabUnselected (Tab tab, FragmentTransaction transaction)
		{
		}

		public void OnTabReselected (Tab tab, FragmentTransaction transaction)
		{
		}
	}
}

