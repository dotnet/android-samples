/**
 * Copyright 2010-present Facebook.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
//
// C# Port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.
// The same license as above applies.
//
using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using Xamarin.FacebookBinding;
using Xamarin.FacebookBinding.Model;
using Xamarin.FacebookBinding.Widget;

[assembly:Permission (Name = Android.Manifest.Permission.Internet)]
[assembly:Permission (Name = Android.Manifest.Permission.WriteExternalStorage)]
[assembly:MetaData ("com.facebook.sdk.ApplicationId", Value ="@string/app_id")]

namespace HelloFacebookSample
{
	[Activity (Label = "@string/app_name", MainLauncher = true, WindowSoftInputMode = SoftInput.AdjustResize)]
	public class HelloFacebookSampleActivity : FragmentActivity
	{
		public HelloFacebookSampleActivity ()
		{
			callback = new MyStatusCallback (this);
		}

		private static readonly string[] PERMISSIONS = new String [] { "publish_actions" };
		private static readonly Location SEATTLE_LOCATION = new Location ("") {
			Latitude = (47.6097),
			Longitude = (-122.3331)
		};
		readonly String PENDING_ACTION_BUNDLE_KEY = "com.facebook.samples.hellofacebook:PendingAction";
		private Button postStatusUpdateButton;
		private Button postPhotoButton;
		private Button pickFriendsButton;
		private Button pickPlaceButton;
		private LoginButton loginButton;
		private ProfilePictureView profilePictureView;
		private TextView greeting;
		private PendingAction pendingAction = PendingAction.NONE;
		private ViewGroup controlsContainer;
		private IGraphUser user;

		enum PendingAction
		{
			NONE,
			POST_PHOTO,
			POST_STATUS_UPDATE
		}

		private UiLifecycleHelper uiHelper;

		class MyStatusCallback : Java.Lang.Object, Session.IStatusCallback
		{
			HelloFacebookSampleActivity owner;

			public MyStatusCallback (HelloFacebookSampleActivity owner)
			{
				this.owner = owner;
			}

			public void Call (Session session, SessionState state, Java.Lang.Exception exception)
			{
				owner.OnSessionStateChange (session, state, exception);
			}
		}

		private Session.IStatusCallback callback;

		class MyUserInfoChangedCallback : Java.Lang.Object, LoginButton.IUserInfoChangedCallback
		{
			HelloFacebookSampleActivity owner;

			public MyUserInfoChangedCallback (HelloFacebookSampleActivity owner)
			{
				this.owner = owner;
			}

			public void OnUserInfoFetched (IGraphUser user)
			{
				owner.user = user;
				owner.UpdateUI ();
				// It's possible that we were waiting for this.user to be populated in order to post a
				// status update.
				owner.HandlePendingAction ();
			}
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			uiHelper = new UiLifecycleHelper (this, callback);
			uiHelper.OnCreate (savedInstanceState);

			if (savedInstanceState != null) {
				string name = savedInstanceState.GetString (PENDING_ACTION_BUNDLE_KEY);
				pendingAction = (PendingAction)Enum.Parse (typeof(PendingAction), name);
			}

			SetContentView (Resource.Layout.main);

			loginButton = (LoginButton)FindViewById (Resource.Id.login_button);
			loginButton.UserInfoChangedCallback = new MyUserInfoChangedCallback (this);

			profilePictureView = FindViewById<ProfilePictureView> (Resource.Id.profilePicture);
			greeting = FindViewById<TextView> (Resource.Id.greeting);

			postStatusUpdateButton = FindViewById<Button> (Resource.Id.postStatusUpdateButton);
			postStatusUpdateButton.Click += delegate {
				OnClickPostStatusUpdate ();
			};

			postPhotoButton = (Button)FindViewById (Resource.Id.postPhotoButton);
			postPhotoButton.Click += delegate {
				OnClickPostPhoto ();
			};

			pickFriendsButton = (Button)FindViewById (Resource.Id.pickFriendsButton);
			pickFriendsButton.Click += delegate {
				OnClickPickFriends ();
			};

			pickPlaceButton = (Button)FindViewById (Resource.Id.pickPlaceButton);
			pickPlaceButton.Click += delegate {
				OnClickPickPlace ();
			};

			controlsContainer = (ViewGroup)FindViewById (Resource.Id.main_ui_container);

			Android.Support.V4.App.FragmentManager fm = SupportFragmentManager;
			Android.Support.V4.App.Fragment fragment = fm.FindFragmentById (Resource.Id.fragment_container);
			if (fragment != null) {
				// If we're being re-created and have a fragment, we need to a) hide the main UI controls and
				// b) hook up its listeners again.
				controlsContainer.Visibility = ViewStates.Gone;
				if (fragment is FriendPickerFragment) {
					SetFriendPickerListeners ((FriendPickerFragment)fragment);
				} else if (fragment is PlacePickerFragment) {
					SetPlacePickerListeners ((PlacePickerFragment)fragment);
				}
			}

			fm.BackStackChanged += delegate {
				if (fm.BackStackEntryCount == 0) {
					// We need to re-show our UI.
					controlsContainer.Visibility = ViewStates.Visible;
				}
			};
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			uiHelper.OnResume ();

			UpdateUI ();
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
			uiHelper.OnSaveInstanceState (outState);

			outState.PutString (PENDING_ACTION_BUNDLE_KEY, pendingAction.ToString ());
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);
			uiHelper.OnActivityResult (requestCode, (int)resultCode, data);
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			uiHelper.OnPause ();
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			uiHelper.OnDestroy ();
		}

		private void OnSessionStateChange (Session session, SessionState state, Exception exception)
		{
			if (pendingAction != PendingAction.NONE &&
				(exception is FacebookOperationCanceledException ||
				exception is FacebookAuthorizationException)) {
				new AlertDialog.Builder (this)
					.SetTitle (Resource.String.cancelled)
						.SetMessage (Resource.String.permission_not_granted)
						.SetPositiveButton (Resource.String.ok, (object sender, DialogClickEventArgs e) => {})
						.Show ();
				pendingAction = PendingAction.NONE;
			} else if (state == SessionState.OpenedTokenUpdated) {
				HandlePendingAction ();
			}
			UpdateUI ();
		}

		private void UpdateUI ()
		{
			Session session = Session.ActiveSession;
			bool enableButtons = (session != null && session.IsOpened);

			postStatusUpdateButton.Enabled = (enableButtons);
			postPhotoButton.Enabled = (enableButtons);
			pickFriendsButton.Enabled = (enableButtons);
			pickPlaceButton.Enabled = (enableButtons);

			if (enableButtons && user != null) {
				profilePictureView.ProfileId = (user.Id);
				greeting.Text = GetString (Resource.String.hello_user, new Java.Lang.String (user.FirstName));
			} else {
				profilePictureView.ProfileId = (null);
				greeting.Text = (null);
			}
		}

		private void HandlePendingAction ()
		{
			PendingAction previouslyPendingAction = pendingAction;
			// These actions may re-set pendingAction if they are still pending, but we assume they
			// will succeed.
			pendingAction = PendingAction.NONE;

			switch (previouslyPendingAction) {
			case PendingAction.POST_PHOTO:
				PostPhoto ();
				break;
			case PendingAction.POST_STATUS_UPDATE:
				PostStatusUpdate ();
				break;
			}
		}

		private void ShowPublishResult (String message, IGraphObject result, FacebookRequestError error)
		{
			String title = null;
			String alertMessage = null;
			if (error == null) {
				title = GetString (Resource.String.success);
				var cls = Java.Lang.Class.ForName ("hellofacebooksample.HelloFacebookSampleAcvitity_GraphObjectWithId");
				var obj = (Java.Lang.Object) result.Cast (cls);
				Java.Lang.Reflect.Method m = obj.Class.GetMethod ("getId");
				String id = (String) m.Invoke (obj);
				alertMessage = GetString (Resource.String.successfully_posted_post, message, id);
			} else {
				title = GetString (Resource.String.error);
				alertMessage = error.ErrorMessage;
			}

			new AlertDialog.Builder (this)
				.SetTitle (title)
					.SetMessage (alertMessage)
					.SetPositiveButton (Resource.String.ok, (object sender, DialogClickEventArgs e) => {})
					.Show ();
		}

		private void OnClickPostStatusUpdate ()
		{
			PerformPublish (PendingAction.POST_STATUS_UPDATE);
		}

		class RequestCallback : Java.Lang.Object, Request.ICallback
		{
			Action<Response> action;

			public RequestCallback (Action<Response> action)
			{
				this.action = action;
			}

			public void OnCompleted (Response response)
			{
				action (response);
			}
		}

		private void PostStatusUpdate ()
		{
			if (user != null && HasPublishPermission ()) {
				string message = GetString (Resource.String.status_update, user.FirstName, (DateTime.Now.ToString ()));
				Request request = Request.NewStatusUpdateRequest (Session.ActiveSession, message, new RequestCallback (response => ShowPublishResult (message, response.GraphObject, response.Error)));
				request.ExecuteAsync ();
			} else {
				pendingAction = PendingAction.POST_STATUS_UPDATE;
			}
		}

		private void OnClickPostPhoto ()
		{
			PerformPublish (PendingAction.POST_PHOTO);
		}

		private void PostPhoto ()
		{
			if (HasPublishPermission ()) {
				Bitmap image = BitmapFactory.DecodeResource (this.Resources, Resource.Drawable.icon);
				Request request = Request.NewUploadPhotoRequest (Session.ActiveSession, image, new RequestCallback (response => {
					ShowPublishResult (GetString (Resource.String.photo_post), response.GraphObject, response.Error);
				}));
				request.ExecuteAsync ();
			} else {
				pendingAction = PendingAction.POST_PHOTO;
			}
		}

		class ErrorListener : Java.Lang.Object, PickerFragment.IOnErrorListener
		{
			Action<PickerFragment, FacebookException> action;

			public ErrorListener (Action<PickerFragment, FacebookException> action)
			{
				this.action = action;
			}

			public void OnError (PickerFragment p0, FacebookException p1)
			{
				action (p0, p1);
			}
		}

		private void ShowPickerFragment (PickerFragment fragment)
		{
			fragment.OnErrorListener = new ErrorListener ((f, e) => {
				String text = GetString (Resource.String.exception, e.Message);
				Toast toast = Toast.MakeText (this, text, ToastLength.Short);
				toast.Show ();
			});

			Android.Support.V4.App.FragmentManager fm = SupportFragmentManager;
			fm.BeginTransaction ()
				.Replace (Resource.Id.fragment_container, fragment)
					.AddToBackStack (null)
					.Commit ();

			controlsContainer.Visibility = ViewStates.Gone;

			// We want the fragment fully created so we can use it immediately.
			fm.ExecutePendingTransactions ();

			fragment.LoadData (false);
		}

		private void OnClickPickFriends ()
		{
			FriendPickerFragment fragment = new FriendPickerFragment ();

			SetFriendPickerListeners (fragment);

			ShowPickerFragment (fragment);
		}

		private void SetFriendPickerListeners (FriendPickerFragment fragment)
		{
			fragment.DoneButtonClicked += delegate {
				OnFriendPickerDone (fragment);
			};
		}

		private void OnFriendPickerDone (FriendPickerFragment fragment)
		{
			Android.Support.V4.App.FragmentManager fm = SupportFragmentManager;
			fm.PopBackStack ();

			String results = "";

			var selection = fragment.Selection;
			if (selection != null && selection.Count > 0) {
				List<String> names = new List<String> ();
				foreach (IGraphUser user in selection) {
					names.Add (user.Name);
				}
				results = string.Join (", ", names.ToArray ());
			} else {
				results = GetString (Resource.String.no_friends_selected);
			}

			ShowAlert (GetString (Resource.String.you_picked), results);
		}

		private void OnPlacePickerDone (PlacePickerFragment fragment)
		{
			Android.Support.V4.App.FragmentManager fm = SupportFragmentManager;
			fm.PopBackStack ();

			String result = "";

			IGraphPlace selection = fragment.Selection;
			if (selection != null) {
				result = selection.Name;
			} else {
				result = GetString (Resource.String.no_place_selected);
			}

			ShowAlert (GetString (Resource.String.you_picked), result);
		}

		private void OnClickPickPlace ()
		{
			PlacePickerFragment fragment = new PlacePickerFragment ();
			fragment.Location = (SEATTLE_LOCATION);
			fragment.TitleText = (GetString (Resource.String.pick_seattle_place));

			SetPlacePickerListeners (fragment);

			ShowPickerFragment (fragment);
		}

		private void SetPlacePickerListeners (PlacePickerFragment fragment)
		{
			fragment.DoneButtonClicked += delegate {
				OnPlacePickerDone (fragment);
			};
			fragment.SelectionChanged += delegate {
				if (fragment.Selection != null) {
					OnPlacePickerDone (fragment);
				}
			};
		}

		private void ShowAlert (String title, String message)
		{
			new AlertDialog.Builder (this)
				.SetTitle (title)
					.SetMessage (message)
					.SetPositiveButton (Resource.String.ok, (object sender, DialogClickEventArgs e) => {})
					.Show ();
		}

		private bool HasPublishPermission ()
		{
			Session session = Session.ActiveSession;
			return session != null && session.Permissions.Contains ("publish_actions");
		}

		private void PerformPublish (PendingAction action)
		{
			Session session = Session.ActiveSession;
			if (session != null) {
				pendingAction = action;
				if (HasPublishPermission ()) {
					// We can do the action right away.
					HandlePendingAction ();
				} else {
					// We need to get new permissions, then complete the action when we get called back.
					session.RequestNewPublishPermissions (new Session.NewPermissionsRequest (this, PERMISSIONS));
				}
			}
		}
	}
}
