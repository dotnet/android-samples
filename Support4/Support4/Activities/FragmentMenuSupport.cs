using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;

namespace Support4
{
	[Activity (Label = "@string/fragment_menu_support")]
	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.support4demo.sample" })]
	public class FragmentMenuSupport : FragmentActivity
	{
		Fragment fragment1;
	    Fragment fragment2;
	    CheckBox checkBox1;
	    CheckBox checkBox2;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			SetContentView(Resource.Layout.fragment_menu);
			
			var fm = SupportFragmentManager;
			var ft = fm.BeginTransaction();
			fragment1 = fm.FindFragmentByTag("f1");
			if(fragment1 == null)
			{
				fragment1 = new MenuFragment();	
				ft.Add (fragment1, "f1");
			}
			fragment2 = fm.FindFragmentByTag("f2");
			if (fragment2 == null) 
			{
				fragment2 = new Menu2Fragment();
				ft.Add(fragment2, "f2");
			}
			ft.Commit();
			
			// Watch check box clicks.
	        checkBox1 = FindViewById<CheckBox>(Resource.Id.menu1);
			checkBox1.Click += HandleCheckBoxClick;
			checkBox2 = FindViewById<CheckBox>(Resource.Id.menu2);
			checkBox2.Click += HandleCheckBoxClick;

        	// Make sure fragments start out with correct visibility.
        	UpdateFragmentVisibility();
		}

		void HandleCheckBoxClick (object sender, EventArgs e)
		{
			UpdateFragmentVisibility();
		}
		
		void UpdateFragmentVisibility()
		{
			var ft = SupportFragmentManager.BeginTransaction();
			if(checkBox1.Checked) ft.Show (fragment1);
			else ft.Hide(fragment1);
			if(checkBox2.Checked) ft.Show (fragment2);
			else ft.Hide(fragment2);
			ft.Commit();
		}
		
		protected class MenuFragment : Fragment
		{
			public override void OnCreate (Bundle p0)
			{
				base.OnCreate (p0);
				
				SetHasOptionsMenu(true);
			}
			
			public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
			{
				base.OnCreateOptionsMenu (menu, inflater);
				
				IMenuItem item;
				item = menu.Add("Menu 1a");
				MenuCompat.SetShowAsAction(item, MenuItemCompat.ShowAsActionIfRoom);
				item = menu.Add("Menu 1b");
				MenuCompat.SetShowAsAction(item, MenuItemCompat.ShowAsActionIfRoom);
			}
		}
		
		protected class Menu2Fragment : Fragment
		{
			public override void OnCreate (Bundle p0)
			{
				base.OnCreate (p0);
				
				SetHasOptionsMenu(true);
			}
			
			public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
			{
				base.OnCreateOptionsMenu (menu, inflater);
				
				IMenuItem item;
				item = menu.Add("Menu 2");
				MenuCompat.SetShowAsAction(item, MenuItemCompat.ShowAsActionIfRoom);
			}
		}
		
	}
}

