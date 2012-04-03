using System;
using System.Collections.Generic;
using System.Json;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using Java.IO;
using Java.Net;
using Com.Facebook.Android;
using Object = Java.Lang.Object;

namespace Com.Facebook.Android
{
	public abstract class BaseDialogListener : Object, Facebook.IDialogListener
	{
		public abstract void OnComplete (Bundle bundle);
		
		public void OnFacebookError (FacebookError e)
		{
			e.PrintStackTrace ();
		}

		public void OnError (DialogError e)
		{
			e.PrintStackTrace ();        
		}

		public void OnCancel ()
		{        
		}
	}

	public abstract class BaseRequestListener : Object, AsyncFacebookRunner.IRequestListener
	{

		public void OnFacebookError (FacebookError e, Object state)
		{
			Log.Error ("Facebook", e.Message);
			e.PrintStackTrace ();
		}

		public void OnFileNotFoundException (FileNotFoundException e,
                                        Object state)
		{
			Log.Error ("Facebook", e.Message);
			e.PrintStackTrace ();
		}

		public void OnIOException (Java.IO.IOException e, Object state)
		{
			Log.Error ("Facebook", e.Message);
			e.PrintStackTrace ();
		}

		public void OnMalformedURLException (MalformedURLException e,
                                        Object state)
		{
			Log.Error ("Facebook", e.Message);
			e.PrintStackTrace ();
		}    

		public abstract void OnComplete (string response, Java.Lang.Object state);
	}

	public class LoginButton : ImageButton
	{
    
		private Facebook mFb;
		private Handler mHandler;
		private SessionListener mSessionListener;
		private String[] mPermissions;
		private Activity mActivity;
    
		public LoginButton (Context context)
			: base (context)
		{
			mSessionListener = new SessionListener (this);
		}
    
		public LoginButton (Context context, IAttributeSet attrs)
			: base (context, attrs)
		{
			mSessionListener = new SessionListener (this);
		}
    
		public LoginButton (Context context, IAttributeSet attrs, int defStyle)
			: base (context, attrs, defStyle)
		{
			mSessionListener = new SessionListener (this);
		}
    
		public void Init (Activity activity, Facebook fb)
		{
			Init (activity, fb, new String[] {});
		}
    
		public void Init (Activity activity, Facebook fb, String[] permissions)
		{
			mActivity = activity;
			mFb = fb;
			mPermissions = permissions;
			mHandler = new Handler ();
        
			SetBackgroundColor (Color.Transparent);
			SetAdjustViewBounds (true);
			SetImageResource (fb.IsSessionValid ?
                         Resource.Drawable.logout_button : 
                         Resource.Drawable.login_button);
			DrawableStateChanged ();
        
			SessionEvents.AddAuthListener (mSessionListener);
			SessionEvents.AddLogoutListener (mSessionListener);
			SetOnClickListener (new ButtonOnClickListener (this));
		}
    
		class ButtonOnClickListener : Object, IOnClickListener
		{
			public ButtonOnClickListener (LoginButton parent)
			{
				this.parent = parent;
			}
			LoginButton parent;
        
			public void OnClick (View arg0)
			{
				if (parent.mFb.IsSessionValid) {
					SessionEvents.OnLogoutBegin ();
					AsyncFacebookRunner asyncRunner = new AsyncFacebookRunner (parent.mFb);
					asyncRunner.Logout (parent.Context, new LogoutRequestListener (parent));
				} else {
					parent.mFb.Authorize (parent.mActivity, parent.mPermissions,
                              new LoginDialogListener ());
				}
			}
		}

		class LoginDialogListener : Object, Facebook.IDialogListener
		{
			public void OnComplete (Bundle values)
			{
				SessionEvents.OnLoginSuccess ();
			}

			public void OnFacebookError (FacebookError error)
			{
				SessionEvents.OnLoginError (error.Message);
			}
        
			public void OnError (DialogError error)
			{
				SessionEvents.OnLoginError (error.Message);
			}

			public void OnCancel ()
			{
				SessionEvents.OnLoginError ("Action Canceled");
			}
		}
    
		private class LogoutRequestListener : BaseRequestListener
		{
			public LogoutRequestListener (LoginButton parent)
			{
				this.parent = parent;
			}
			
			LoginButton parent;
			
			public override void OnComplete (String response, Object state)
			{
				// callback should be run in the original thread, 
				// not the background thread
				parent.mHandler.Post (delegate {
					SessionEvents.OnLogoutFinish ();
				});
			}
		}
    
		class SessionListener : Object, SessionEvents.IAuthListener, SessionEvents.ILogoutListener
		{        
			public SessionListener (LoginButton parent)
			{
				this.parent = parent;
			}
			
			LoginButton parent;
			
			public void OnAuthSucceed ()
			{
				parent.SetImageResource (Resource.Drawable.logout_button);
				SessionStore.Save (parent.mFb, parent.Context);
			}

			public void OnAuthFail (String error)
			{
			}
        
			public void OnLogoutBegin ()
			{           
			}
        
			public void OnLogoutFinish ()
			{
				SessionStore.Clear (parent.Context);
				parent.SetImageResource (Resource.Drawable.login_button);
			}
		}    
	}

	public class SessionEvents
	{

		private static LinkedList<IAuthListener> mAuthListeners = 
        new LinkedList<IAuthListener> ();
		private static LinkedList<ILogoutListener> mLogoutListeners = 
        new LinkedList<ILogoutListener> ();

		/**
     * Associate the given listener with this Facebook object. The listener's
     * callback interface will be invoked when authentication events occur.
     * 
     * @param listener
     *            The callback object for notifying the application when auth
     *            events happen.
     */
		public static void AddAuthListener (IAuthListener listener)
		{
			mAuthListeners.AddLast (listener);
		}

		/**
     * Remove the given listener from the list of those that will be notified
     * when authentication events occur.
     * 
     * @param listener
     *            The callback object for notifying the application when auth
     *            events happen.
     */
		public static void RemoveAuthListener (IAuthListener listener)
		{
			mAuthListeners.Remove (listener);
		}

		/**
     * Associate the given listener with this Facebook object. The listener's
     * callback interface will be invoked when logout occurs.
     * 
     * @param listener
     *            The callback object for notifying the application when log out
     *            starts and finishes.
     */
		public static void AddLogoutListener (ILogoutListener listener)
		{
			mLogoutListeners.AddLast (listener);
		}

		/**
     * Remove the given listener from the list of those that will be notified
     * when logout occurs.
     * 
     * @param listener
     *            The callback object for notifying the application when log out
     *            starts and finishes.
     */
		public static void RemoveLogoutListener (ILogoutListener listener)
		{
			mLogoutListeners.Remove (listener);
		}
    
		public static void OnLoginSuccess ()
		{
			foreach (var listener in mAuthListeners) {
				listener.OnAuthSucceed ();
			}
		}
    
		public static void OnLoginError (String error)
		{
			foreach (var listener in mAuthListeners) {
				listener.OnAuthFail (error);
			}
		}
    
		public static void OnLogoutBegin ()
		{
			foreach (var l in mLogoutListeners) {
				l.OnLogoutBegin ();
			}
		}
    
		public static void OnLogoutFinish ()
		{
			foreach (var l in mLogoutListeners) {
				l.OnLogoutFinish ();
			}   
		}
    
		/**
     * Callback interface for authorization events.
     *
     */
		public interface IAuthListener
		{

			/**
         * Called when a auth flow completes successfully and a valid OAuth 
         * Token was received.
         * 
         * Executed by the thread that initiated the authentication.
         * 
         * API requests can now be made.
         */
			void OnAuthSucceed ();

			/**
         * Called when a login completes unsuccessfully with an error. 
         *  
         * Executed by the thread that initiated the authentication.
         */
			void OnAuthFail (String error);
		}
    
		/**
     * Callback interface for logout events.
     *
     */ 
		public interface ILogoutListener
		{
			/**
         * Called when logout begins, before session is invalidated.  
         * Last chance to make an API call.  
         * 
         * Executed by the thread that initiated the logout.
         */
			void OnLogoutBegin ();

			/**
         * Called when the session information has been cleared.
         * UI should be updated to reflect logged-out state.
         * 
         * Executed by the thread that initiated the logout.
         */
			void OnLogoutFinish ();
		}
    
	}

	public class SessionStore
	{    
		const string TOKEN = "access_token";
		const string EXPIRES = "expires_in";
		const string KEY = "facebook-session";
    
		public static bool Save (Facebook session, Context context)
		{
			var editor = context.GetSharedPreferences (KEY,FileCreationMode.Private).Edit ();
			editor.PutString (TOKEN, session.AccessToken);
			editor.PutLong (EXPIRES, session.AccessExpires);
			return editor.Commit ();
		}

		public static bool Restore (Facebook session, Context context)
		{
			var savedSession = context.GetSharedPreferences (KEY, FileCreationMode.Private);
			session.AccessToken = savedSession.GetString (TOKEN, null);
			session.AccessExpires = savedSession.GetLong (EXPIRES, 0);
			return session.IsSessionValid;
		}

		public static void Clear (Context context)
		{
			var editor = context.GetSharedPreferences (KEY, FileCreationMode.Private).Edit ();
			editor.Clear ();
			editor.Commit ();
		}    
	}
	
	[Activity (Label = "simple", MainLauncher = true)]
	public class Example : Activity
	{

		// Your Facebook Application ID must be set before running this example
		// See http://www.facebook.com/developers/createapp.php
		const String APP_ID = "175729095772478";
		private LoginButton mLoginButton;
		private TextView mText;
		private Button mRequestButton;
		private Button mPostButton;
		private Button mDeleteButton;
		private Button mUploadButton;
		private Facebook mFacebook;
		private AsyncFacebookRunner mAsyncRunner;

		/** Called when the activity is first created. */
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			if (APP_ID == null) {
				Util.ShowAlert (this, "Warning", "Facebook Applicaton ID must be " +
                    "specified before running this example: see Example.java");
			}

			SetContentView (Resource.Layout.main);
			mLoginButton = (LoginButton) FindViewById (Resource.Id.login);
			mText = (TextView) FindViewById (Resource.Id.txt);
			mRequestButton = (Button) FindViewById (Resource.Id.requestButton);
			mPostButton = (Button) FindViewById (Resource.Id.postButton);
			mDeleteButton = (Button) FindViewById (Resource.Id.deletePostButton);
			mUploadButton = (Button) FindViewById (Resource.Id.uploadButton);

			mFacebook = new Facebook (APP_ID);
			mAsyncRunner = new AsyncFacebookRunner (mFacebook);

			SessionStore.Restore (mFacebook, this);
			SessionEvents.AddAuthListener (new SampleAuthListener (this));
			SessionEvents.AddLogoutListener (new SampleLogoutListener (this));
			mLoginButton.Init (this, mFacebook);

			mRequestButton.Click += delegate {
				mAsyncRunner.Request ("me", new SampleRequestListener (this));
			};
			mRequestButton.Visibility = mFacebook.IsSessionValid ?
                ViewStates.Visible :
                ViewStates.Invisible;

			mUploadButton.Click += delegate {
				Bundle parameters = new Bundle ();
				parameters.PutString ("method", "photos.upload");

				URL uploadFileUrl = null;
				try {
					uploadFileUrl = new URL (
                        "http://www.facebook.com/images/devsite/iphone_connect_btn.jpg");
				} catch (MalformedURLException e) {
					e.PrintStackTrace ();
				}
				try {
					HttpURLConnection conn = (HttpURLConnection) uploadFileUrl.OpenConnection ();
					conn.DoInput = true;
					conn.Connect ();
					int length = conn.ContentLength;

					byte[] imgData = new byte[length];
					var ins = conn.InputStream;
					ins.Read (imgData, 0, imgData.Length);
					parameters.PutByteArray ("picture", imgData);

				} catch (IOException e) {
					e.PrintStackTrace ();
				}

				mAsyncRunner.Request (null, parameters, "POST",
                        new SampleUploadListener (this), null);
			};
			mUploadButton.Visibility = mFacebook.IsSessionValid ?
                ViewStates.Visible :
                ViewStates.Invisible;

			mPostButton.Click += delegate {
				mFacebook.Dialog (this, "feed",
                        new SampleDialogListener (this));
			};
			mPostButton.Visibility = mFacebook.IsSessionValid ?
                ViewStates.Visible :
                ViewStates.Invisible;
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			mFacebook.AuthorizeCallback (requestCode, (int) resultCode, data);
		}

		public class SampleAuthListener : SessionEvents.IAuthListener
		{
			public SampleAuthListener (Example parent)
			{
				this.parent = parent;
			}
			Example parent;

			public void OnAuthSucceed ()
			{
				parent.mText.Text = ("You have logged in! ");
				parent.mRequestButton.Visibility = (ViewStates.Visible);
				parent.mUploadButton.Visibility = (ViewStates.Visible);
				parent.mPostButton.Visibility = (ViewStates.Visible);
			}

			public void OnAuthFail (String error)
			{
				parent.mText.Text = ("Login Failed: " + error);
			}
		}

		public class SampleLogoutListener : SessionEvents.ILogoutListener
		{
			public SampleLogoutListener (Example parent)
			{
				this.parent = parent;
			}
			Example parent;

			public void OnLogoutBegin ()
			{
				parent.mText.Text = ("Logging out...");
			}

			public void OnLogoutFinish ()
			{
				parent.mText.Text = ("You have logged out! ");
				parent.mRequestButton.Visibility = (ViewStates.Invisible);
				parent.mUploadButton.Visibility = (ViewStates.Invisible);
				parent.mPostButton.Visibility = (ViewStates.Invisible);
			}
		}

		public class SampleRequestListener : BaseRequestListener
		{
			public SampleRequestListener (Example parent)
			{
				this.parent = parent;
			}
			Example parent;


			public override void OnComplete (String response, Object state)
			{
				try {
					// process the response here: executed in background thread
					Log.Debug ("Facebook-Example", "Response: " + response);
					var json = (JsonObject) JsonValue.Parse (response);
					String name = json ["name"];

					// then post the processed result back to the UI thread
					// if we do not do this, an runtime exception will be generated
					// e.g. "CalledFromWrongThreadException: Only the original
					// thread that created a view hierarchy can touch its views."
					parent.RunOnUiThread (delegate {
						parent.mText.Text = ("Hello there, " + name + "!");
					});
				//} catch (JSONException e) {
				//	Log.Warn ("Facebook-Example", "JSON Error in response");
				} catch (FacebookError e) {
					Log.Warn ("Facebook-Example", "Facebook Error: " + e.Message);
				}
			}
		}

		public class SampleUploadListener : BaseRequestListener
		{
			public SampleUploadListener (Example parent)
			{
				this.parent = parent;
			}
			Example parent;


			public override void OnComplete (String response, Object state)
			{
				try {
					// process the response here: (executed in background thread)
					Log.Debug ("Facebook-Example", "Response: " + response.ToString ());
					var json = (JsonObject) JsonValue.Parse (response);
					String src = json ["src"];

					// then post the processed result back to the UI thread
					// if we do not do this, an runtime exception will be generated
					// e.g. "CalledFromWrongThreadException: Only the original
					// thread that created a view hierarchy can touch its views."
					parent.RunOnUiThread (delegate {
						parent.mText.Text = ("Hello there, photo has been uploaded at \n" + src);
					});
				//} catch (JSONException e) {
				//	Log.Warn ("Facebook-Example", "JSON Error in response");
				} catch (FacebookError e) {
					Log.Warn ("Facebook-Example", "Facebook Error: " + e.Message);
				}
			}
		}

		public class WallPostRequestListener : BaseRequestListener
		{
			public WallPostRequestListener (Example parent)
			{
				this.parent = parent;
			}
			Example parent;


			public override void OnComplete (String response, Object state)
			{
				Log.Debug ("Facebook-Example", "Got response: " + response);
				String message = "<empty>";
				try {
					var json = (JsonObject) JsonValue.Parse (response);
					message = json ["message"];
				//} catch (JSONException e) {
				//	Log.Warn ("Facebook-Example", "JSON Error in response");
				} catch (FacebookError e) {
					Log.Warn ("Facebook-Example", "Facebook Error: " + e.Message);
				}
				String text = "Your Wall Post: " + message;
				parent.RunOnUiThread (delegate {
					parent.mText.Text = (text);
				});
			}
		}

		public class WallPostDeleteListener : BaseRequestListener
		{
			public WallPostDeleteListener (Example parent)
			{
				this.parent = parent;
			}
			Example parent;


			public override void OnComplete (String response, Object state)
			{
				if (response.Equals ("true")) {
					Log.Debug ("Facebook-Example", "Successfully deleted wall post");
					parent.RunOnUiThread (delegate {
						parent.mDeleteButton.Visibility = (ViewStates.Invisible);
						parent.mText.Text = ("Deleted Wall Post");
					});
				} else {
					Log.Debug ("Facebook-Example", "Could not delete wall post");
				}
			}
		}

		public class SampleDialogListener : BaseDialogListener
		{
			public SampleDialogListener (Example parent)
			{
				this.parent = parent;
			}
			Example parent;


			public override void OnComplete (Bundle values)
			{
				String postId = values.GetString ("post_id");
				if (postId != null) {
					Log.Debug ("Facebook-Example", "Dialog Success! post_id=" + postId);
					parent.mAsyncRunner.Request (postId, new WallPostRequestListener (parent));
					parent.mDeleteButton.Click += delegate {
						parent.mAsyncRunner.Request (postId, new Bundle (), "DELETE",
                                new WallPostDeleteListener (parent), null);
					};
					parent.mDeleteButton.Visibility = (ViewStates.Visible);
				} else {
					Log.Debug ("Facebook-Example", "No wall post made");
				}
			}
		}

	}
}


