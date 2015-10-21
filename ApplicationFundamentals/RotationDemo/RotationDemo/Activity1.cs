using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace RotationDemo
{
	[Activity (Label = "RotationDemo", MainLauncher = true)] //, ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation)]
    public class Activity1 : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);
		}
		
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			menu.Add (Menu.None, 0, Menu.None, Resource.String.simpleSaveState);
			menu.Add (Menu.None, 1, Menu.None, Resource.String.nonConfigInstance);
			menu.Add (Menu.None, 2, Menu.None, Resource.String.layoutInCode);

			return base.OnCreateOptionsMenu (menu);
		}
		
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			Intent intent = null;
			
			switch (item.ItemId) {
			case 0:
				intent = new Intent (this, typeof(SimpleStateActivity));
				break;
			case 1:
				intent = new Intent (this, typeof(NonConfigInstanceActivity));
				break;
			case 2:
				intent = new Intent (this, typeof(CodeLayoutActivity));
				break;
			}
			
			StartActivity (intent);
			
			return base.OnOptionsItemSelected (item);
		}
	}
}