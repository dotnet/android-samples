/*
 * Copyright (C) 2014 The Android Open Source Project
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

using System.IO;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Webkit;
using CommonSampleLibrary;

namespace PermissionRequest
{
	/// <summary>
	/// This fragment shows a <see cref="Android.Webkit.WebView"/>.
	/// </summary>
	public class PermissionRequestFragment : ConfirmationDialogFragment, Listener
	{
		static string TAG = typeof(PermissionRequestFragment).Name;

		const string FRAGMENT_DIALOG = "dialog";

		/// <summary>
		/// Stores the content of sample.html once loaded
		/// </summary>
		string mHtmlContent = "";

		/// <summary>
		/// A refrence to the <see cref="Android.Webkit.WebView"/>.
		/// </summary>
		WebView mWebView;

		/// <summary>
		/// This field stores the <see cref="Android.Webkit.PermissionRequest"/> from the web application 
		/// until it is allowed or denied by user.
		/// </summary>
		Android.Webkit.PermissionRequest mPermissionRequest;

		/// <summary>
		/// For testing.
		/// </summary>
		ConsoleMonitor mConsoleMonitor;

		/// <summary>
		/// This <see cref="Android.Webkit.WebChromeClient"/> has implementation for handling 
		/// <see cref="Android.Webkit.PermissionRequest"/>.
		/// </summary>
		MyWebChromeClient mWebChromeClient;

		public PermissionRequestFragment ()
		{
			mWebChromeClient = new MyWebChromeClient (this);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.fragment_permission_request, container, false);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			mWebView = (WebView)view.FindViewById (Resource.Id.web_view);
			// Here, we use #mWebChromeClient with implementation for handling PermissionRequests.
			mWebView.SetWebChromeClient (mWebChromeClient);
			ConfigureWebSettings (mWebView.Settings);
		}

		public override void OnResume ()
		{
			base.OnResume ();

			if (string.IsNullOrWhiteSpace (mHtmlContent)) {
				mHtmlContent = System.Text.Encoding.UTF8.GetString (OpenAsset ("sample.html")); 
			}

			mWebView.LoadDataWithBaseURL ("file:///android_asset/", mHtmlContent, "text/html", "utf-8", null);
		}

		byte[] OpenAsset (string filename)
		{
			Stream input = null;
			var output = new MemoryStream ();
			try {
				input = Resources.Assets.Open (filename);
				byte[] buffer = new byte[1024];
				int size;
				while ((size = input.Read (buffer, 0, buffer.Length)) > 0) {
					output.Write (buffer, 0, size);
				}
				output.Flush ();
				return output.ToArray ();
			} catch (System.IO.FileNotFoundException) {
				Log.Error (TAG, "Unable to read the contents of " + filename);
				return null;
			} finally {
				if (null != input) {
					input.Close ();
				}
			}
		}

		static void ConfigureWebSettings (WebSettings settings)
		{
			settings.JavaScriptEnabled = true;
			settings.AllowFileAccessFromFileURLs = true;
			settings.AllowUniversalAccessFromFileURLs = true;
		}

		class MyWebChromeClient : WebChromeClient
		{
			readonly PermissionRequestFragment permissionReqFrag;

			public MyWebChromeClient (PermissionRequestFragment permissionRequestFragment)
			{
				permissionReqFrag = permissionRequestFragment;
			}

			/// <summary>
			/// Called when the web content is requesting permission to access some resources.
			/// </summary>
			/// <param name="request">Request.</param>
			public override void OnPermissionRequest (Android.Webkit.PermissionRequest request)
			{
				Log.Info (PermissionRequestFragment.TAG, "onPermissionRequest");
				permissionReqFrag.mPermissionRequest = request;
				ConfirmationDialogFragment.NewInstance (request.GetResources ())
					.Show (permissionReqFrag.ChildFragmentManager, FRAGMENT_DIALOG);
			}

			/// <summary>
			/// Called when the permission request is canceled by the web content.
			/// </summary>
			/// <param name="request">Request.</param>
			public override void OnPermissionRequestCanceled (Android.Webkit.PermissionRequest request)
			{
				Log.Info (TAG, "onPermissionRequestCanceled");
				// We dismiss the prompt UI here as the request is no longer valid.
				permissionReqFrag.mPermissionRequest = null;
				DialogFragment fragment = (DialogFragment)permissionReqFrag.ChildFragmentManager
					.FindFragmentByTag (FRAGMENT_DIALOG);
				if (null != fragment) {
					fragment.Dismiss ();
				}
			}

			public override bool OnConsoleMessage (ConsoleMessage consoleMessage)
			{
				var messageLvl = consoleMessage.InvokeMessageLevel (); 
				if (messageLvl.Equals (ConsoleMessage.MessageLevel.Tip))
					Log.Verbose (TAG, consoleMessage.Message ());
				if (messageLvl.Equals (ConsoleMessage.MessageLevel.Log))
					Log.Info (TAG, consoleMessage.Message ());
				if (messageLvl.Equals (ConsoleMessage.MessageLevel.Warning))
					Log.Warn (TAG, consoleMessage.Message ());
				if (messageLvl.Equals (ConsoleMessage.MessageLevel.Error))
					Log.Error (TAG, consoleMessage.Message ());
				if (messageLvl.Equals (ConsoleMessage.MessageLevel.Debug))
					Log.Debug (TAG, consoleMessage.Message ());
				
				if (null != permissionReqFrag.mConsoleMonitor) {
					permissionReqFrag.mConsoleMonitor.OnConsoleMessage (consoleMessage);
				}
				return true;
			}
		}

		public void OnConfirmation (bool allowed)
		{
			if (allowed) {
				mPermissionRequest.Grant (mPermissionRequest.GetResources ());
				Log.Debug (TAG, "Permission granted.");
			} else {
				mPermissionRequest.Deny ();
				Log.Debug (TAG, "Permission request denied.");
			}
			mPermissionRequest = null;
		}

		public void SetConsoleMonitor (ConsoleMonitor monitor)
		{
			mConsoleMonitor = monitor;
		}

		/// <summary>
		/// For testing.
		/// </summary>
		public interface ConsoleMonitor
		{
			void OnConsoleMessage (ConsoleMessage message);
		}
	}
}

