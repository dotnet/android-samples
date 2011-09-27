/*
 * Copyright (C) 2011 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace com.example.monodroid.hcgallery
{
	public class MainActivity : Activity, ActionBar.ITabListener
	{
		private const int NotificationDefault = 1;
		private const string ActionDialog = "com.example.monodroid.hcgallery.action.DIALOG";
		
		private View mActionBarView;
		private Animator mCurrentTitlesAnimator;
		private string [] mToggleLabels = {"Show Titles", "Hide Titles"};
		private int mLabelIndex = 1;
		private int mThemeId = -1;

		protected override void OnCreate (Bundle savedInstanceState) 
		{
			base.OnCreate (savedInstanceState);

			if (savedInstanceState != null && savedInstanceState.GetInt ("theme", -1) != -1) {
				mThemeId = savedInstanceState.GetInt ("theme");
				this.SetTheme (mThemeId);
			}

			SetContentView (Resource.Layout.main);
			
			Directory.InitializeDirectory ();
			
			ActionBar bar = ActionBar;
			
			int i;
			for (i = 0; i < Directory.CategoryCount; i++) {
				bar.AddTab (bar.NewTab ().SetText (Directory.GetCategory (i).Name)
					.SetTabListener (this));
			}

			mActionBarView = LayoutInflater.Inflate (
				Resource.Layout.action_bar_custom, null);

			bar.CustomView = mActionBarView;
			bar.DisplayOptions = ActionBarDisplayOptions.ShowCustom | ActionBarDisplayOptions.UseLogo;
			bar.NavigationMode = ActionBarNavigationMode.Tabs;
			bar.SetDisplayShowHomeEnabled (true);

			// If category is not saved to the savedInstanceState,
			// 0 is returned by default.
			if (savedInstanceState != null) {
				int category = savedInstanceState.GetInt ("category");
				bar.SelectTab (bar.GetTabAt (category));
			}
		}

		public void OnTabSelected (ActionBar.Tab tab, FragmentTransaction ft) 
		{
			TitlesFragment titleFrag = (TitlesFragment) FragmentManager.FindFragmentById (Resource.Id.frag_title);
			titleFrag.PopulateTitles (tab.Position);
		
			titleFrag.SelectPosition (0);
		}

		public void OnTabUnselected (ActionBar.Tab tab, FragmentTransaction ft)
		{
		}

		public void OnTabReselected (ActionBar.Tab tab, FragmentTransaction ft)
		{
		}

		public override bool OnCreateOptionsMenu (IMenu menu) 
		{
			MenuInflater inflater = MenuInflater;
			inflater.Inflate (Resource.Menu.main_menu, menu);
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.camera:
				Intent intent = new Intent (this, typeof (CameraSample));
				intent.PutExtra ("theme", mThemeId);
				StartActivity (intent);
				return true;
				
			case Resource.Id.toggleTitles:
				ToggleVisibleTitles ();
				return true;
				
			case Resource.Id.toggleTheme:
				if (mThemeId == Resource.Style.AppTheme_Dark) {
					mThemeId = Resource.Style.AppTheme_Light;
				} else {
					mThemeId = Resource.Style.AppTheme_Dark;
				}
				this.Recreate ();
				return true;
				
			case Resource.Id.showDialog:
				ShowDialog ("This is indeed an awesome dialog.");
				return true;
				
			case Resource.Id.showStandardNotification:
				ShowNotification (false);
				return true;
				
			case Resource.Id.showCustomNotification:
				ShowNotification (true);
				return true;
				
			default:
				return base.OnOptionsItemSelected(item);
			}
		}
		
		class AnimatorUpdateListener : Java.Lang.Object, ValueAnimator.IAnimatorUpdateListener
		{
			bool isPortrait;
			View titlesView;
			ViewGroup.LayoutParams lp;
			public AnimatorUpdateListener (bool isPortrait, View titlesView, ViewGroup.LayoutParams lp)
			{
				this.isPortrait = isPortrait;
				this.titlesView = titlesView;
				this.lp = lp;
			}

			public void OnAnimationUpdate (ValueAnimator valueAnimator)
			{
				// *** WARNING ***: triggering layout at each animation frame highly impacts
				// performance so you should only do this for simple layouts. More complicated
				// layouts can be better served with individual animations on child views to
				// avoid the performance penalty of layout.
				if (isPortrait) {
					lp.Height = (int) (Java.Lang.Integer) valueAnimator.AnimatedValue;
				} else {
					lp.Width = (int) (Java.Lang.Integer) valueAnimator.AnimatedValue;
				}
				titlesView.LayoutParameters = lp;
			}
		}
		
		class ObjectAnimatorListenerAdapter : AnimatorListenerAdapter
		{
			MainActivity parent;
			public ObjectAnimatorListenerAdapter (MainActivity parent)
			{
				this.parent = parent;
			}
			
			public override void OnAnimationEnd (Animator animator) 
			{
				parent.mCurrentTitlesAnimator = null;
			}
		}

		class ObjectAnimatorListenerAdapter2 : AnimatorListenerAdapter
		{
			MainActivity parent;
			FragmentManager fm;
			TitlesFragment f;
			
			public ObjectAnimatorListenerAdapter2 (MainActivity parent, FragmentManager fm, TitlesFragment f)
			{
				this.parent = parent;
				this.fm = fm;
				this.f = f;
			}
			
			bool canceled;
	
			public override void OnAnimationCancel (Animator animation)
			{
				canceled = true;
				base.OnAnimationCancel (animation);
			}
	
			public override void OnAnimationEnd (Animator animator)
			{
				if (canceled)
					return;
				parent.mCurrentTitlesAnimator = null;
				fm.BeginTransaction ().Hide (f).Commit ();
			}
		}
		
		public void ToggleVisibleTitles () 
		{
			// Use these for custom animations.
			FragmentManager fm = FragmentManager;
			TitlesFragment f = (TitlesFragment) fm.FindFragmentById (Resource.Id.frag_title);
			View titlesView = f.View;
			mLabelIndex = 1 - mLabelIndex;
			
			// Determine if we're in portrait, and whether we're showing or hiding the titles
			// with this toggle.
			bool isPortrait = Resources.Configuration.Orientation ==
				Android.Content.Res.Orientation.Portrait;
			
			bool shouldShow = f.IsHidden || mCurrentTitlesAnimator != null;
			
			// Cancel the current titles animation if there is one.
			if (mCurrentTitlesAnimator != null)
				mCurrentTitlesAnimator.Cancel ();
			
			// Begin setting up the object animator. We'll animate the bottom or right edge of the
			// titles view, as well as its alpha for a fade effect.
			ObjectAnimator objectAnimator = ObjectAnimator.OfPropertyValuesHolder (
				titlesView,
				PropertyValuesHolder.OfInt (
					isPortrait ? "bottom" : "right",
					shouldShow ? Resources.GetDimensionPixelSize (Resource.Dimension.titles_size)
				 : 0),
				PropertyValuesHolder.OfFloat ("alpha", shouldShow ? 1 : 0)
				);
			
			// At each step of the animation, we'll perform layout by calling setLayoutParams.
			ViewGroup.LayoutParams lp = titlesView.LayoutParameters;
			objectAnimator.AddUpdateListener (new AnimatorUpdateListener (isPortrait, titlesView, lp));
			
			if (shouldShow) {
				fm.BeginTransaction ().Show (f).Commit ();
				objectAnimator.AddListener (new ObjectAnimatorListenerAdapter (this));
			} else {
				objectAnimator.AddListener (new ObjectAnimatorListenerAdapter2 (this, fm, f));
			}
			
			// Start the animation.
			objectAnimator.Start ();
			mCurrentTitlesAnimator = objectAnimator;
			
			InvalidateOptionsMenu ();
			
			// Manually trigger onNewIntent to check for ACTION_DIALOG.
			OnNewIntent (Intent);
		}

		protected override void OnNewIntent (Intent intent) 
		{
			if (ActionDialog == intent.Action) {
				ShowDialog (intent.GetStringExtra (Intent.ExtraText));
			}
		}

		void ShowDialog (String text) 
		{
			// DialogFragment.show() will take care of adding the fragment
			// in a transaction.  We also want to remove any currently showing
			// dialog, so make our own transaction and take care of that here.
			FragmentTransaction ft = FragmentManager.BeginTransaction ();
			
			DialogFragment newFragment = MyDialogFragment.NewInstance (text);
			
			// Show the dialog.
			newFragment.Show (ft, "dialog");
		}

		void ShowNotification (bool custom) 
		{
			Resources res = Resources;
			NotificationManager notificationManager = (NotificationManager) GetSystemService (
				NotificationService);
			
			Notification.Builder builder = new Notification.Builder (this)
				.SetSmallIcon (Resource.Drawable.ic_stat_notify_example)
				.SetAutoCancel (true)
				.SetTicker (GetString(Resource.String.notification_text))
				.SetContentIntent (GetDialogPendingIntent ("Tapped the notification entry."));
				
			if (custom) {
				// Sets a custom content view for the notification, including an image button.
				RemoteViews layout = new RemoteViews (PackageName, Resource.Layout.notification);
				layout.SetTextViewText (Resource.Id.notification_title, GetString (Resource.String.app_name));
				layout.SetOnClickPendingIntent (Resource.Id.notification_button,
					GetDialogPendingIntent ("Tapped the 'dialog' button in the notification."));
				builder.SetContent (layout);
				
				// Notifications in Android 3.0 now have a standard mechanism for displaying large
				// bitmaps such as contact avatars. Here, we load an example image and resize it to the
				// appropriate size for large bitmaps in notifications.
				Bitmap largeIconTemp = BitmapFactory.DecodeResource (res,
					Resource.Drawable.notification_default_largeicon);
				Bitmap largeIcon = Bitmap.CreateScaledBitmap (
					largeIconTemp,
					res.GetDimensionPixelSize (Android.Resource.Dimension.NotificationLargeIconWidth),
					res.GetDimensionPixelSize (Android.Resource.Dimension.NotificationLargeIconHeight),
					false);
				largeIconTemp.Recycle ();
				
				builder.SetLargeIcon (largeIcon);
				
			} else {
				builder
				.SetNumber (7) // An example number.
				.SetContentTitle (GetString (Resource.String.app_name))
				.SetContentText (GetString (Resource.String.notification_text));
			}
				
			notificationManager.Notify (NotificationDefault, builder.Notification);
		}

		PendingIntent GetDialogPendingIntent (String dialogText) 
		{
			return PendingIntent.GetActivity (
				this,
				dialogText.GetHashCode (), // Otherwise previous PendingIntents with the same
												// requestCode may be overwritten.
				new Intent (ActionDialog)
					.PutExtra (Intent.ExtraText, dialogText)
					.AddFlags (ActivityFlags.NewTask),
				0);
			}

		public override bool OnPrepareOptionsMenu (IMenu menu) 
		{
			menu.GetItem (1).SetTitle (new Java.Lang.String (mToggleLabels [mLabelIndex]));
			return true;
		}

		protected override void OnSaveInstanceState (Bundle outState) 
		{
			base.OnSaveInstanceState (outState);
			ActionBar bar = ActionBar;
			int category = bar.SelectedTab.Position;
			outState.PutInt ("category", category);
			outState.PutInt ("theme", mThemeId);
		}

		public class MyDialogFragment : DialogFragment 
		{
			public static MyDialogFragment NewInstance (String title) 
			{
				MyDialogFragment frag = new MyDialogFragment ();
				Bundle args = new Bundle ();
				args.PutString ("text", title);
				frag.Arguments = args;
				return frag;
			}

			public override Dialog OnCreateDialog(Bundle savedInstanceState) {
				String text = Arguments.GetString ("text");
					
				return new AlertDialog.Builder (Activity)
					.SetTitle ("A Dialog of Awesome")
					.SetMessage (text)
					.SetPositiveButton (Android.Resource.String.Ok, delegate (object o, DialogClickEventArgs e) {})
					.Create();
			}
		}
	}
}
