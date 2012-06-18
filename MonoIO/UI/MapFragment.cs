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
using Android.Webkit;
using Android.Support.V4.App;
using Android.Util;
using Uri = Android.Net.Uri;
using MonoIO.UI;
using Fragment = Android.Support.V4.App.Fragment;
using Java.Interop;

namespace MonoIO
{
	public class MapFragment : Fragment
	{
		protected static string TAG = "MapFragment";

		/**
	     * When specified, will automatically point the map to the requested room.
	     */
		public static string EXTRA_ROOM = "monoio.extra.ROOM";
		protected static string MAP_JSI_NAME = "MAP_CONTAINER";
		protected static string MAP_URL = "http://www.google.com/events/io/2011/embed.html";
		protected static bool CLEAR_CACHE_ON_LOAD = false;
		protected static WebView webView;
		protected static View loadingSpinner;
		protected static bool mapInitialized = false;
		private WebChromeClient webChromeClient = new MyWebChromeClient ();
		private WebViewClient webViewClient = new MyWebViewClient ();
		
		public override void OnCreate (Bundle p0)
		{
			base.OnCreate (p0);
			SetHasOptionsMenu (true);
			//AnalyticsUtils.getInstance(getActivity()).trackPageView("/Map");
		}
		
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			
			ViewGroup root = (ViewGroup)inflater.Inflate (Resource.Layout.fragment_webview_with_spinner, null);

			// For some reason, if we omit this, NoSaveStateFrameLayout thinks we are
			// FILL_PARENT / WRAP_CONTENT, making the progress bar stick to the top of the activity.
			root.LayoutParameters = new ViewGroup.LayoutParams (ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent);
	
			loadingSpinner = root.FindViewById (Resource.Id.loading_spinner);
			webView = root.FindViewById<WebView> (Resource.Id.webview);
			webView.SetWebChromeClient (webChromeClient);
			webView.SetWebViewClient (webViewClient);
	
			webView.Post (() => {
				if (CLEAR_CACHE_ON_LOAD) {
					webView.ClearCache (true);	
				}
				webView.Settings.JavaScriptEnabled = true;
				webView.Settings.JavaScriptCanOpenWindowsAutomatically = false;
				webView.LoadUrl (MAP_URL);
				webView.AddJavascriptInterface (new MyMapJsi (Activity, savedInstanceState), MAP_JSI_NAME);
			});
			
			return root;
			
		}
		
		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			base.OnCreateOptionsMenu (menu, inflater);
			inflater.Inflate (Resource.Menu.refresh_menu_items, menu);
		}
		
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (item.ItemId == Resource.Id.menu_refresh) {
				webView.Reload ();
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}
		
		protected static void RunJs (String js)
		{
			if (Log.IsLoggable (TAG, LogPriority.Debug)) {
				Log.Debug (TAG, "Loading javascript:" + js);
			}
			webView.LoadUrl ("javascript:" + js);
		}
		
		/**
	     * Helper method to escape JavaScript strings. Useful when passing strings to a WebView via
	     * "javascript:" calls.
	     */
		protected static string EscapeJsString (String s)
		{
			if (s == null) {
				return "";
			}
	
			return s.Replace ("'", "\\'").Replace ("\"", "\\\"");
		}
		
		public void PanLeft (float screenFraction)
		{
			RunJs ("IoMap.panLeft('" + screenFraction + "');");
		}
		
		private class MyWebChromeClient : WebChromeClient
		{
			public override void OnConsoleMessage (String message, int lineNumber, String sourceID)
			{
				Log.Info (TAG, "JS Console message: (" + sourceID + ": " + lineNumber + ") " + message);
			}
		}
		
		private class MyWebViewClient : WebViewClient
		{
			public override void OnPageStarted (WebView view, string url, Android.Graphics.Bitmap favicon)
			{
				base.OnPageStarted (view, url, favicon);
				loadingSpinner.Visibility = ViewStates.Visible;
				webView.Visibility = ViewStates.Invisible;
			}
			
			public override void OnPageFinished (WebView view, string url)
			{
				base.OnPageFinished (view, url);
				loadingSpinner.Visibility = ViewStates.Gone;
				webView.Visibility = ViewStates.Visible;
			}
			
			public override void OnReceivedError (WebView view, ClientError errorCode, string description, string failingUrl)
			{
				Log.Error (TAG, "Error " + errorCode + ": " + description);
				Toast.MakeText (view.Context, "Error " + errorCode + ": " + description, ToastLength.Long).Show ();
				base.OnReceivedError (view, errorCode, description, failingUrl);
			}
		}
		
		/**
	     * I/O Conference Map JavaScript interface.
	     */
		private interface MapJsi
		{
			void OpenContentInfo (String roomId);

			void OnMapReady ();
		}
		
		public class MyMapJsi : Java.Lang.Object, MapJsi
		{
			private Activity _activity;
			private Bundle _arguments;
			
			public MyMapJsi (Activity activity, Bundle arguments)
			{
				_activity = activity;	
			}
		
			[Export("openContentInfo")]
			public void OpenContentInfo (String roomId)
			{
				var possibleTrackId = ParserUtils.TranslateTrackIdAlias (roomId);
				Intent intent = new Intent ();
				if (ParserUtils.LOCAL_TRACK_IDS.Contains (possibleTrackId)) {
					// This is a track; open up the sandbox for the track, since room IDs that are
					// track IDs are sandbox areas in the map.
					Uri trackVendorsUri = ScheduleContract.Tracks.BuildVendorsUri (possibleTrackId);
					intent = new Intent (Intent.ActionView, trackVendorsUri);
				} else {
					Uri roomUri = ScheduleContract.Rooms.BuildSessionsDirUri (roomId);
					intent = new Intent (Intent.ActionView, roomUri);
				}
				
				_activity.RunOnUiThread (() => {
					(_activity as BaseActivity).OpenActivityOrFragment (intent);	
				}); 
			}
	
			[Export("onMapReady")]
			public void OnMapReady ()
			{
				if (Log.IsLoggable (TAG, LogPriority.Debug)) {
					Log.Debug (TAG, "onMapReady");
				}
	
				Intent intent = BaseActivity.FragmentArgumentsToIntent (_arguments);
	
				string showRoomId = null;
				if (!mapInitialized && intent.HasExtra (EXTRA_ROOM)) {
					showRoomId = intent.GetStringExtra (EXTRA_ROOM);
				}
	
				if (showRoomId != null) {
					
					RunJs ("IoMap.showLocationById('" + EscapeJsString (showRoomId) + "');");
				}
	
				mapInitialized = true;
			}
		}
	}
}

