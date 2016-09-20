/**
 * Copyright (c) 2014-present, Facebook, Inc. All rights reserved.
 *
 * You are hereby granted a non-exclusive, worldwide, royalty-free license to use,
 * copy, modify, and distribute this software in source code or binary form for use
 * in connection with the web services and APIs provided by Facebook.
 *
 * As with any software that integrates with the Facebook platform, your use of
 * this software is subject to the Facebook Developer Principles and Policies
 * [http://developers.facebook.com/policy/]. This copyright notice shall be
 * included in all copies or substantial portions of the software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */


using System;
using System.Collections.Generic;
using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;
using Xamarin.Facebook.Share;
using Xamarin.Facebook.Share.Model;
using Xamarin.Facebook.Share.Widget;
using Object = Java.Lang.Object;
using Uri = Android.Net.Uri;

// NOTE: Facebook SDK rquires that the 'Value' point to a string resource
//       in your values/ folder (eg: strings.xml file).
//       It will not allow you to use the app_id value directly here!
[assembly: MetaData("com.facebook.sdk.ApplicationId", Value = "@string/app_id")]
[assembly: MetaData("com.facebook.sdk.ApplicationName", Value = "@string/facebook_app_name")]
[assembly: Permission(Name = Manifest.Permission.Internet)]
[assembly: Permission(Name = Manifest.Permission.WriteExternalStorage)]

namespace HelloFacebookSample
{
    [Activity(Label = "@string/app_name", MainLauncher = true, WindowSoftInputMode = SoftInput.AdjustResize)]
    public class HelloFacebookSampleActivity : FragmentActivity
    {

        private static readonly string[] PERMISSION = { "publish_actions" };
        private static readonly Location SEATTLE_LOCATION = new Location("")
        {
            Latitude = (47.6097),
            Longitude = (-122.3331)
        };

        readonly string PENDING_ACTION_BUNDLE_KEY = "com.facebook.samples.hellofacebook:PendingAction";

        Button postStatusUpdateButton;
        Button postPhotoButton;
        ProfilePictureView profilePictureView;
        TextView greeting;
        PendingAction pendingAction = PendingAction.NONE;
        bool canPresentShareDialog;
        bool canPresentShareDialogWithPhotos;
        ICallbackManager callbackManager;
        ProfileTracker profileTracker;
        ShareDialog shareDialog;
        IFacebookCallback shareCallback;

        enum PendingAction
        {
            NONE,
            POST_PHOTO,
            POST_STATUS_UPDATE
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Initialize the SDK before executing any other operations
            FacebookSdk.SdkInitialize(Application.Context);

            // create callback manager using CallbackManagerFactory
            callbackManager = CallbackManagerFactory.Create();

            LoginManager.Instance.RegisterCallback(callbackManager, new MyFacebookCallback<LoginResult>(this));

            shareDialog = new ShareDialog(this);
            shareCallback = new MySharedDialogCallback<SharerResult>(this);
            shareDialog.RegisterCallback(callbackManager, shareCallback);

            if (savedInstanceState != null)
            {
                var name = savedInstanceState.GetString(PENDING_ACTION_BUNDLE_KEY);
                System.Enum.TryParse(name, true, out pendingAction);
            }

            SetContentView(Resource.Layout.main);

            profileTracker = new MyProfileTracker(this);

            profilePictureView = FindViewById<ProfilePictureView>(Resource.Id.profilePicture);
            greeting = FindViewById<TextView>(Resource.Id.greeting);

            postStatusUpdateButton = FindViewById<Button>(Resource.Id.postStatusUpdateButton);
            postStatusUpdateButton.Click += (sender, args) =>
            {
                OnClickPostStatusUpdate();
            };

            postPhotoButton = FindViewById<Button>(Resource.Id.postPhotoButton);
            postPhotoButton.Click += (sender, args) =>
            {
                OnClickPostPhoto();
            };

            // Can we present the share dialog for regular links?
            canPresentShareDialog = ShareDialog.CanShow(Class.FromType(typeof(ShareLinkContent)));

            // Can we present the share dialog for photos?
            canPresentShareDialogWithPhotos = ShareDialog.CanShow(Class.FromType(typeof(SharePhotoContent)));
        }

        protected override void OnResume()
        {
            base.OnResume();
            UpdateUI();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString(PENDING_ACTION_BUNDLE_KEY, pendingAction.ToString());
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            callbackManager.OnActivityResult(requestCode, Convert.ToInt32(resultCode), data);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            profileTracker.StopTracking();
        }

        private void UpdateUI()
        {
            bool enableButtons = AccessToken.CurrentAccessToken != null;

            postStatusUpdateButton.Enabled = enableButtons || canPresentShareDialog;
            postPhotoButton.Enabled = enableButtons || canPresentShareDialogWithPhotos;

            var profile = Profile.CurrentProfile;
            if (enableButtons && profile != null)
            {
                profilePictureView.ProfileId = profile.Id;
                greeting.Text = GetString(Resource.String.hello_user, profile.FirstName);
            }
            else
            {
                profilePictureView.ProfileId = null;
                greeting.Text = null;
            }
        }

        private void HandlePendingAction()
        {
            PendingAction previouslyPendingAction = pendingAction;
            // These actions may re-set pendingAction if they are still pending, but we assume they
            // will succeed.
            pendingAction = PendingAction.NONE;

            switch (previouslyPendingAction)
            {
                case PendingAction.NONE:
                    break;
                case PendingAction.POST_PHOTO:
                    PostPhoto();
                    break;
                case PendingAction.POST_STATUS_UPDATE:
                    PostStatusUpdate();
                    break;
            }
        }

        private void OnClickPostStatusUpdate()
        {
            PerformPublish(PendingAction.POST_STATUS_UPDATE, canPresentShareDialog);

            /*
            var parameters = new Bundle();
            parameters.PutString("message", "This is a test message");
            var request = new GraphRequest(currentAccessToken, "/me/feed", parameters, HttpMethod.Post,
                new MyGraphPostCallback(this));
            request.ExecuteAsync();
            */
        }

        private void PostStatusUpdate()
        {
            var profile = Profile.CurrentProfile;
            ShareLinkContent linkContent = new ShareLinkContent.Builder()
                .SetContentTitle("Hello Facebook")
                .SetContentDescription("The 'Hello Facebook' sample  showcases simple Facebook integration")
                .SetImageUrl(Uri.Parse("http://developers.facebook.com/docs/android"))
                .Build();
            if (canPresentShareDialog)
            {
                shareDialog.Show(linkContent);
            }
            else if (profile != null && HasPublishPermission())
            {
                ShareApi.Share(linkContent, shareCallback);
            }
            else
            {
                pendingAction = PendingAction.POST_STATUS_UPDATE;
            }
        }

        private void OnClickPostPhoto()
        {
            PerformPublish(PendingAction.POST_PHOTO, canPresentShareDialogWithPhotos);

            /*
            //  Use a photo that is already on the internet by publishing using the url parameter
            var parameters = new Bundle();
            parameters.PutString("url", "https://blog.xamarin.com/wp-content/uploads/2015/03/RDXWoY7W_400x400.png");
            var request = new GraphRequest(currentAccessToken, "/me/photos", parameters, HttpMethod.Post,
                new MyGraphPostCallback(this));
            request.ExecuteAsync();
            */
        }

        private void PostPhoto()
        {
            Bitmap image = BitmapFactory.DecodeResource(Resources, Resource.Drawable.icon);
            SharePhoto sharePhoto = (SharePhoto) new SharePhoto.Builder().SetBitmap(image).Build();
            IList<SharePhoto> photos = new List<SharePhoto>();
            photos.Add(sharePhoto);

            SharePhotoContent sharePhotoContent =
                    new SharePhotoContent.Builder().SetPhotos(photos).Build();
            if (canPresentShareDialogWithPhotos)
            {
                shareDialog.Show(sharePhotoContent);
            }
            else if (HasPublishPermission())
            {
                ShareApi.Share(sharePhotoContent, shareCallback);
            }
            else
            {
                pendingAction = PendingAction.POST_PHOTO;
                // We need to get new permissions, then complete the action when we get called back.
                LoginManager.Instance.LogInWithPublishPermissions(this, PERMISSION);
            }
        }


        private bool HasPublishPermission()
        {
            var accessToken = AccessToken.CurrentAccessToken;
            return accessToken != null && accessToken.Permissions.Contains("publish_actions");
        }

        private void PerformPublish(PendingAction action, bool allowNoToken)
        {
            AccessToken accessToken = AccessToken.CurrentAccessToken;
            if (accessToken != null || allowNoToken)
            {
                pendingAction = action;
                HandlePendingAction();
            }
        }

        //
        // Inner Class section
        //

        private class MySharedDialogCallback<SharerResult> : Java.Lang.Object, IFacebookCallback
            where SharerResult : Java.Lang.Object
        {

            private HelloFacebookSampleActivity owner;

            public MySharedDialogCallback(HelloFacebookSampleActivity owner)
            {
                this.owner = owner;
            }

            public void OnCancel()
            {
                Log.Debug("HelloFacebook", "Canceled");
            }

            public void OnError(FacebookException error)
            {
                Log.Debug("HelloFacebook", string.Format("Error: %s", error.Message.ToString()));
                var title = owner.GetString(Resource.String.error);
                var alertMessage = error.Message;
                ShowResult(title, alertMessage);
            }

            public void OnSuccess(Object obj)
            {
                Log.Debug("HelloFacebook", "Success!");
                var result = (Xamarin.Facebook.Share.SharerResult) obj;
                if (result.PostId != null)
                {
                    string title = owner.GetString(Resource.String.success);
                    string id = result.PostId;
                    string alertMessage = owner.GetString(Resource.String.successfully_posted_post, id);
                    ShowResult(title, alertMessage);
                }
            }

            private void ShowResult(string title, string alertMessage)
            {
                IDialogInterfaceOnClickListener listener = null;
                new AlertDialog.Builder(owner)
                    .SetTitle(title)
                    .SetMessage(alertMessage)
                    .SetPositiveButton(Resource.String.ok, listener)
                    .Show();
            }
        }

        class MyProfileTracker : ProfileTracker
        {

            readonly HelloFacebookSampleActivity owner;

            public MyProfileTracker(HelloFacebookSampleActivity owner)
            {
                this.owner = owner;
            }

            protected override void OnCurrentProfileChanged(Profile oldProfile, Profile newProfile)
            {
                owner.UpdateUI();
                // It's possible that we were waiting for Profile to be populated in order to
                // post a status update.
                owner.HandlePendingAction();
            }
        }

        class MyFacebookCallback<LoginResult> : Java.Lang.Object, IFacebookCallback where LoginResult : Java.Lang.Object
        {

            readonly HelloFacebookSampleActivity owner;

            public MyFacebookCallback(HelloFacebookSampleActivity owner)
            {
                this.owner = owner;
            }

            public void OnSuccess(Java.Lang.Object obj)
            {
                owner.HandlePendingAction();
                owner.UpdateUI();
            }

            public void OnCancel()
            {
                if (owner.pendingAction != PendingAction.NONE)
                {
                    ShowAlert();
                    owner.pendingAction = PendingAction.NONE;
                }
                owner.UpdateUI();
            }

            public void OnError(FacebookException fbException)
            {
                if (owner.pendingAction != PendingAction.NONE
                                && fbException.Class.Equals(Java.Lang.Class.FromType(typeof(FacebookAuthorizationException)))) {
                    ShowAlert();
                    owner.pendingAction = PendingAction.NONE;
                }
                owner.UpdateUI();
            }

            private void ShowAlert()
            {
                IDialogInterfaceOnClickListener listener = null;
                new AlertDialog.Builder(owner)
                    .SetTitle(Resource.String.cancelled)
                    .SetMessage(Resource.String.permission_not_granted)
                    .SetPositiveButton(Resource.String.ok, listener)
                    .Show();
            }
        }
    }
} 