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
using Android.Util;
using Android.Support.V4.View;
using Android.Support.V4.View.Accessibility;

namespace Support4
{
	[Activity (Label = "@string/accessibility_delegate_title", Theme = "@style/ThemeHolo")]
	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.support4demo.sample" })]
	public class AccessibilityDelegateSupportActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			SetContentView(Resource.Layout.accessibility_delegate);
		}
		
		/**
	     * This class represents a View that is customized via an AccessibilityDelegate
	     * as opposed to inheritance. An accessibility delegate can be used for adding
	     * accessibility to custom Views, i.e. ones that extend classes from android.view,
	     * in a backwards compatible fashion. Note that overriding a method whose return
	     * type or arguments are not part of a target platform APIs makes your application
	     * not backwards compatible with that platform version.
	     */
		
		public class AccessibilityDelegateSupportView : View 
		{
	        public AccessibilityDelegateSupportView(Context context, IAttributeSet attrs) : base(context, attrs)
			{
	            InstallAccessibilityDelegate();
	        }
	
	        private void InstallAccessibilityDelegate() 
			{
	            // The accessibility delegate enables customizing accessibility behavior
	            // via composition as opposed as inheritance. The main benefit is that
	            // one can write a backwards compatible application by setting the delegate
	            // only if the API level is high enough i.e. the delegate is part of the APIs.
	            // The easiest way to achieve that is by using the support library which
	            // takes the burden of checking API version and knowing which API version
	            // introduced the delegate off the developer.
				
				ViewCompat.SetAccessibilityDelegate(this, new MyAccessibilityDelegateCompat(this));
			}
			
			public class MyAccessibilityDelegateCompat : AccessibilityDelegateCompat
			{
				AccessibilityDelegateSupportView parent;
				public MyAccessibilityDelegateCompat (AccessibilityDelegateSupportView parent)
				{
					this.parent = parent;
				}
				
				public override void OnPopulateAccessibilityEvent (View host, Android.Views.Accessibility.AccessibilityEvent e)
				{
					base.OnPopulateAccessibilityEvent (host, e);
					// Note that View.onPopulateAccessibilityEvent was introduced in
	                // ICS and we would like to tweak a bit the text that is reported to
					// accessibility services via the AccessibilityEvent.
					e.Text.Add (new Java.Lang.String (parent.Context.GetString (Resource.String.accessibility_delegate_custom_text_added)));
				}
				
				public override void OnInitializeAccessibilityNodeInfo (View host, AccessibilityNodeInfoCompat info)
				{
					base.OnInitializeAccessibilityNodeInfo (host, info);
					
					// Note that View.onInitializeAccessibilityNodeInfo was introduced in
                    // ICS and we would like to tweak a bit the text that is reported to
                    // accessibility services via the AccessibilityNodeInfo.
					info.Text = parent.Context.GetString (Resource.String.accessibility_delegate_custom_text_added);
				}
			}
        }
	}
}

