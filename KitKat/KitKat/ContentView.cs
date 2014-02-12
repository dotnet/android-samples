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


namespace KitKat
{
	public class ContentView : ScrollView, View.IOnClickListener, View.IOnSystemUiVisibilityChangeListener
	{
		public ContentView (Context context) :
			base (context)
		{
			Initialize ();
		}

		public ContentView (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize ();
		}

		public ContentView (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}

		public void OnClick (View v)
		{
			//SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.ImmersiveSticky | (StatusBarVisibility)SystemUiFlags.Fullscreen | (StatusBarVisibility)SystemUiFlags.HideNavigation;

		}

		public void OnSystemUiVisibilityChange (StatusBarVisibility visibility)
		{
			throw new NotImplementedException ();
		}
	}
}

