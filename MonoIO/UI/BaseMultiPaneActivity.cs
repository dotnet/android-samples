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
using MonoIO.UI;
using Android.Content.PM;
using Android.Support.V4.App;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

namespace MonoIO
{
	public abstract class BaseMultiPaneActivity : BaseActivity
	{
		public override void OpenActivityOrFragment (Android.Content.Intent intent)
		{
			var pm = PackageManager;
			var resolveInfoList = pm.QueryIntentActivities (intent, PackageInfoFlags.MatchDefaultOnly);
			foreach (var resolveInfo in resolveInfoList) {
				FragmentReplaceInfo fri = OnSubstituteFragmentForActivityLaunch (resolveInfo.ActivityInfo.Name);
				if (fri != null) {
					Bundle arguments = IntentToFragmentArguments (intent);
					FragmentManager fm = SupportFragmentManager;
	
					try {
						Fragment fragment = (Fragment)fri.GetFragmentClass ().NewInstance ();
						fragment.Arguments = arguments;
	
						FragmentTransaction ft = fm.BeginTransaction ();
						ft.Replace (fri.GetContainerId (), fragment, fri.GetFragmentTag ());
						OnBeforeCommitReplaceFragment (fm, ft, fragment);
						ft.Commit ();
					} catch (Exception e) {
						new Exception ("Error creating new fragment." + e.Message);
					}
					return;
				}
			}
			base.OpenActivityOrFragment (intent);
		}
		
		/**
	     * Callback that's triggered to find out if a fragment can substitute the given activity class.
	     * Base activites should return a {@link FragmentReplaceInfo} if a fragment can act in place
	     * of the given activity class name.
	     */
		protected virtual FragmentReplaceInfo OnSubstituteFragmentForActivityLaunch (String activityClassName)
		{
			return null;
		}
		
		/**
	     * Called just before a fragment replacement transaction is committed in response to an intent
	     * being fired and substituted for a fragment.
	     */
		protected virtual void OnBeforeCommitReplaceFragment (FragmentManager fm, FragmentTransaction ft, Fragment fragment)
		{
		}
		
		/**
	     * A class describing information for a fragment-substitution, used when a fragment can act
	     * in place of an activity.
	     */
		protected class FragmentReplaceInfo
		{
			private Java.Lang.Class mFragmentClass;
			private String mFragmentTag;
			private int mContainerId;
	
			public FragmentReplaceInfo (Java.Lang.Class fragmentClass, String fragmentTag, int containerId)
			{
				mFragmentClass = fragmentClass;
				mFragmentTag = fragmentTag;
				mContainerId = containerId;
			}
	
			public Java.Lang.Class GetFragmentClass ()
			{
				return mFragmentClass;
			}
	
			public String GetFragmentTag ()
			{
				return mFragmentTag;
			}
	
			public int GetContainerId ()
			{
				return mContainerId;
			}
		}
	}
}

