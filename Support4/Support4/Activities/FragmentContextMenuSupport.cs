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
using Android.Util;

namespace Support4
{
	[Activity (Label = "@string/fragment_context_menu_support")]
	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.support4demo.sample" })]
	public class FragmentContextMenuSupport : FragmentActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create the list fragment and add it as our sole content.
	        ContextMenuFragment content = new ContextMenuFragment();
	        SupportFragmentManager.BeginTransaction().Add(Android.Resource.Id.Content, content).Commit();
		}
		
		public class ContextMenuFragment : Fragment
		{	
			public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle p2)
			{
				var root = inflater.Inflate(Resource.Layout.fragment_context_menu, container, false);
            	RegisterForContextMenu(root.FindViewById(Resource.Id.long_press));
            	return root;
			}
			
			public override void OnCreateContextMenu (IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
			{
				base.OnCreateContextMenu (menu, v, menuInfo);
				
				menu.Add(Menu.None, Resource.Id.a_item, Menu.None, "Menu A");
            	menu.Add(Menu.None, Resource.Id.b_item, Menu.None, "Menu B");
			}
			
			public override bool OnContextItemSelected (IMenuItem item)
			{
				switch (item.ItemId) {
	                case Resource.Id.a_item:
	                    Log.Info("ContextMenu", "Item 1a was chosen");
	                    return true;
	                case Resource.Id.b_item:
	                    Log.Info("ContextMenu", "Item 1b was chosen");
	                    return true;
	            }
				return base.OnContextItemSelected (item);
			}
		}
	}
}

