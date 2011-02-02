using System;
using System.Linq;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Mono.Samples.SkeletonApp
{
	[Activity (MainLauncher = true)]
	public class SkeletonActivity : Activity
	{
		EditText mEditor;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Inflate our UI from its XML layout description
			SetContentView (Resource.Layout.skeleton_activity);

			// Find the text editor view inside the layout, because we
			// want to do various programmatic things with it.
			mEditor = FindViewById<EditText> (Resource.Id.editor);

			// Hook up button presses to the appropriate event handler.
			FindViewById (Resource.Id.back).Click += delegate { Finish (); };
			FindViewById (Resource.Id.clear).Click += delegate { mEditor.Text = String.Empty; };

			mEditor.Text = GetText (Resource.String.main_label);
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			base.OnCreateOptionsMenu (menu);

			// We are going to create two menus. Note that we assign them
			// unique integer IDs, labels from our string resources, and
			// given them shortcuts.

			menu.Add (0, (int)MenuItems.Back, 0, Resource.String.back).SetShortcut ('0', 'b');
			menu.Add (0, (int)MenuItems.Clear, 0, Resource.String.clear).SetShortcut ('1', 'c');

			return true;
		}

		// Called right before your activity's option menu is displayed.
		public override bool OnPrepareOptionsMenu (IMenu menu)
		{
			base.OnPrepareOptionsMenu (menu);

			// Before showing the menu, we need to decide whether the clear
			// item is enabled depending on whether there is text to clear.
			menu.FindItem ((int)MenuItems.Clear).SetVisible (mEditor.Text.Any ());

			return true;
		}

		// Called when a menu item is selected.
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch ((MenuItems)item.ItemId) {
				case MenuItems.Back:
					Finish ();
					return true;
				case MenuItems.Clear:
					mEditor.Text = String.Empty;
					return true;
			}
			
			return base.OnOptionsItemSelected (item);
		}
	}
}
