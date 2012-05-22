using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using System.Text;

using Fragment = Android.Support.V4.App.Fragment;

namespace ViewPagerIndicator
{
	class TestFragment : Fragment
	{
		private const string KEY_CONTENT = "TestFragment:Content";
		string mContent = "???";
		
		public TestFragment ()
		{	
		}
		
		public TestFragment (string content)
		{
	
			var builder = new StringBuilder ();
			for (int i = 0; i < 20; i++) {
				if (i != 19)
					builder.Append (content).Append (" ");
				else
					builder.Append (content);
			}
			mContent = builder.ToString ();

		}
		
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			if ((savedInstanceState != null) && savedInstanceState.ContainsKey (KEY_CONTENT)) {
				mContent = savedInstanceState.GetString (KEY_CONTENT);
			}
	
			TextView text = new TextView (Activity);
			text.Gravity = GravityFlags.Center;
			text.Text = mContent;
			text.TextSize = (20 * Resources.DisplayMetrics.Density);
			text.SetPadding (20, 20, 20, 20);
	
			LinearLayout layout = new LinearLayout (Activity);
			layout.LayoutParameters = new ViewGroup.LayoutParams (ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent);
			layout.SetGravity (GravityFlags.Center);
			layout.AddView (text);
	
			return layout;
		}
		
		public override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
			outState.PutString (KEY_CONTENT, mContent);
		}
	}
}

