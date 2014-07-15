using System;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V4.App;
using Android.Widget;
using Android.Support.V4.View;


namespace AndroidSupportSample
{
	[Android.App.Activity (Label = "@string/action_bar_fragment_menu", Theme = "@style/Theme.AppCompat")]				
	public class ActionBarFragmentMenu : ActionBarActivity
	{

		MenuFragment mFragment1;
		Menu2Fragment mFragment2;
		CheckBox mCheckBox1;
		CheckBox mCheckBox2;
		CheckBox mCheckBox3;
		CheckBox mHasOptionsMenu;
		CheckBox mMenuVisibility;

		void OnClickListener_OnClick (object sender, EventArgs e)
		{
			UpdateFragmentVisibility();
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView(Resource.Layout.action_bar_fragment_menu);

			// Make sure the two menu fragments are created.
			var fm = SupportFragmentManager;
			var ft = fm.BeginTransaction();
			mFragment1 = (MenuFragment)fm.FindFragmentByTag("f1");
			if (mFragment1 == null) {
				mFragment1 = new MenuFragment();
				ft.Add(mFragment1, "f1");
			}
			mFragment2 = (Menu2Fragment)fm.FindFragmentByTag("f2");
			if (mFragment2 == null) {
				mFragment2 = new Menu2Fragment();
				ft.Add(mFragment2, "f2");
			}
			ft.Commit();

			// Watch check box clicks.
			mCheckBox1 = (CheckBox)FindViewById(Resource.Id.menu1);
			mCheckBox1.Click += OnClickListener_OnClick;
			mCheckBox2 = (CheckBox)FindViewById(Resource.Id.menu2);
			mCheckBox2.Click += OnClickListener_OnClick;
			mCheckBox3 = (CheckBox)FindViewById(Resource.Id.menu3);
			mCheckBox3.Click += OnClickListener_OnClick;
			mHasOptionsMenu = (CheckBox)FindViewById(Resource.Id.has_options_menu);
			mHasOptionsMenu.Click += OnClickListener_OnClick;
			mMenuVisibility = (CheckBox)FindViewById(Resource.Id.menu_visibility);
			mMenuVisibility.Click += OnClickListener_OnClick;

			// Make sure fragments start out with correct visibility.
			UpdateFragmentVisibility();
		}

		protected override void OnRestoreInstanceState (Bundle savedInstanceState)
		{
			base.OnRestoreInstanceState (savedInstanceState);
			// Make sure fragments are updated after check box view state is restored.
			UpdateFragmentVisibility();
		}

		// Update fragment visibility based on current check box state.
		void UpdateFragmentVisibility() {
			// Update top level fragments.
			var ft = SupportFragmentManager.BeginTransaction();
			if (mCheckBox1.Checked) ft.Show(mFragment1);
			else ft.Hide(mFragment1);
			if (mCheckBox2.Checked) ft.Show(mFragment2);
			else ft.Hide(mFragment2);
			ft.Commit();

			mFragment1.HasOptionsMenu = mHasOptionsMenu.Checked;
			mFragment1.SetMenuVisibility (mMenuVisibility.Checked);
			mFragment2.HasOptionsMenu = mHasOptionsMenu.Checked;
			mFragment2.SetMenuVisibility (mMenuVisibility.Checked);

			// Update the nested fragment.
			if (mFragment2.mFragment3 != null) {
				ft = mFragment2.FragmentManager.BeginTransaction();
				if (mCheckBox3.Checked) ft.Show(mFragment2.mFragment3);
				else ft.Hide(mFragment2.mFragment3);
				ft.Commit();

				mFragment2.mFragment3.HasOptionsMenu = mHasOptionsMenu.Checked;
				mFragment2.mFragment3.SetMenuVisibility(mMenuVisibility.Checked);
			}
		}

		/**
	     * A fragment that displays a menu.  This fragment happens to not
	     * have a UI (it does not implement onCreateView), but it could also
	     * have one if it wanted.
	     */
		class MenuFragment : Fragment
		{
			public override void OnCreate (Bundle savedInstanceState)
			{
				base.OnCreate (savedInstanceState);
				HasOptionsMenu = true;
			}

			public override void OnCreateOptionsMenu (Android.Views.IMenu menu, Android.Views.MenuInflater inflater)
			{
				MenuItemCompat.SetShowAsAction(menu.Add(new Java.Lang.String ("Menu 1a")), (int)Android.Views.ShowAsAction.IfRoom);
				MenuItemCompat.SetShowAsAction(menu.Add(new Java.Lang.String ("Menu 1b")), (int)Android.Views.ShowAsAction.IfRoom);
				base.OnCreateOptionsMenu (menu, inflater);
			}

			public override bool OnOptionsItemSelected (Android.Views.IMenuItem item)
			{
				if (item.TitleFormatted.ToString () == "Menu 1a") {
					Toast.MakeText(Activity, "Selected Menu 1a.", ToastLength.Short).Show();
					return true;
				}
				if (item.TitleFormatted.ToString () == "Menu 1b") {
					Toast.MakeText(Activity, "Selected Menu 1b.", ToastLength.Short).Show();
					return true;
				}

				return base.OnOptionsItemSelected (item);
			}
		}



		/**
	     * Second fragment with a menu.
	     */
		class Menu2Fragment : Fragment
		{
			public Menu3Fragment mFragment3;

			public override void OnCreate (Bundle savedInstanceState)
			{
				base.OnCreate (savedInstanceState);
				HasOptionsMenu = true;

				var fm = ChildFragmentManager;
				var ft = fm.BeginTransaction();
				mFragment3 = (Menu3Fragment)fm.FindFragmentByTag("f3");
				if (mFragment3 == null) {
					mFragment3 = new Menu3Fragment();
					ft.Add(mFragment3, "f3");
				}
				ft.Commit();
			}

			public override void OnCreateOptionsMenu (Android.Views.IMenu menu, Android.Views.MenuInflater inflater)
			{
				MenuItemCompat.SetShowAsAction(menu.Add(new Java.Lang.String ("Menu 2")), (int)Android.Views.ShowAsAction.IfRoom);
			}

			public override bool OnOptionsItemSelected (Android.Views.IMenuItem item)
			{
				if (item.TitleFormatted.ToString () == "Menu 2") {
					Toast.MakeText (Activity, "Selected Menu 2.", ToastLength.Short).Show ();
					return true;
				}
				return false;	
			}
		}

		/**
	     * Third fragment with a menu.
	     * This one is nested within the second.
	     */
		class Menu3Fragment : Fragment
		{
			public override void OnCreate (Bundle savedInstanceState)
			{
				base.OnCreate (savedInstanceState);
				HasOptionsMenu = true;
			}

			public override void OnCreateOptionsMenu (Android.Views.IMenu menu, Android.Views.MenuInflater inflater)
			{
				Toast.MakeText (Activity, "Created nested fragment's menu.", ToastLength.Short).Show ();
				inflater.Inflate (Resource.Menu.display_options_actions, menu);
				base.OnCreateOptionsMenu (menu, inflater);
			}
	
			public override void OnDestroyOptionsMenu ()
			{
				Toast.MakeText (Activity, "Destroyed nested fragment's menu.", ToastLength.Short).Show ();
				base.OnDestroyOptionsMenu ();
			}

			public override void OnPrepareOptionsMenu (Android.Views.IMenu menu)
			{
				Toast.MakeText (Activity, "Prepared nested fragment's menu.", ToastLength.Short).Show ();
				base.OnPrepareOptionsMenu (menu);
			}

			public override bool OnOptionsItemSelected (Android.Views.IMenuItem item)
			{
				if (item.ItemId == Resource.Id.simple_item) {
					Toast.MakeText (Activity, "Selected nested fragment's menu item.", ToastLength.Short).Show ();
					return true;
				}

				return base.OnOptionsItemSelected (item);
			}
		}
	}
}

