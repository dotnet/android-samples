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

namespace RuntimePermissions
{
	public class RuntimePermissionsFragment : Fragment
	{
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View root = inflater.Inflate (Resource.Layout.fragment_main, null);

			if ((int)Build.VERSION.SdkInt < 23) {
				/*
				 * The contacts permissions have been declared in the AndroidManifest for Android M only.
				 * They are not available on older platforms, so we are hiding the button to access the
				 * contacts database.
				 * The contacts permissions have been declared in the AndroidManifest for Android M  and
				 * above only. They are not available on older platforms, so we are hiding the button to
				 * access the contacts database.
				 * This shows how new runtime-only permissions can be added, that do not apply to older
				 * platform versions. This can be useful for automated updates where additional
				 * permissions might prompt the user on upgrade.
				*/
				root.FindViewById (Resource.Id.button_contacts).Visibility = ViewStates.Gone;
			}

			return root;		
		}
	}
}

