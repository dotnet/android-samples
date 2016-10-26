/*
 * Copyright (C) 2016 The Android Open Source Project
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
using System.Collections.Generic;
using System.IO;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Util;
using Void = Java.Lang.Void;
using Java.Net;
using Java.Util;
using IOException = Java.IO.IOException;
using Uri = Android.Net.Uri;

namespace AppShortcuts
{
	public class ShortcutHelper
	{
		private static string TAG = Main.TAG;

		private static string EXTRA_LAST_REFRESH = "com.example.android.shortcutsample.EXTRA_LAST_REFRESH";

		private static long REFRESH_INTERVAL_MS = 60 * 60 * 1000;

		private Context mContext;

		private ShortcutManager mShortcutManager;

		public ShortcutHelper(Context context)
		{
			mContext = context;
			//mShortcutManager = mContext.getSystemService(ShortcutManager.class);
		}

		public void MaybeRestoreAllDynamicShortcuts()
		{
			if (mShortcutManager.GetDynamicShortcuts().size() == 0)
			{
				// NOTE: If this application is always supposed to have dynamic shortcuts, then publish
				// them here.
				// Note when an application is "restored" on a new device, all dynamic shortcuts
				// will *not* be restored but the pinned shortcuts *will*.
			}
		}

		public void ReportShortcutUsed(string id)
		{
			mShortcutManager.ReportShortcutUsed(id);
		}

		/**
		 * Use this when interacting with ShortcutManager to show consistent error messages.
		 */
		private void CallShortcutManager(BooleanSupplier r)
		{
			try
			{
				if (!r.GetAsBoolean())
				{
					Utils.ShowToast(mContext, "Call to ShortcutManager is rate-limited");
				}
			}
			catch (Exception e)
			{
				Log.Error(TAG, "Caught Exception", e);
				Utils.ShowToast(mContext, "Error while calling ShortcutManager: " + e.ToString());
			}
		}

		/**
		 * Return all mutable shortcuts from this app self.
		 */
		public List<ShortcutInfo> GetShortcuts()
		{
			// Load mutable dynamic shortcuts and pinned shortcuts and put them into a single list
			// removing duplicates.

			List<ShortcutInfo> ret = new List<ShortcutInfo>();
			HashSet<string> seenKeys = new HashSet<string>();

			// Check existing shortcuts shortcuts
			foreach (var shortcut in mShortcutManager.getDynamicShortcuts())
			{
				if (shortcut.IsImmutable()) continue;
				ret.Add(shortcut);
				seenKeys.Add(shortcut.GetId());
			}

			foreach (var shortcut in mShortcutManager.getPinnedShortcuts())
			{
				if (shortcut.IsImmutable() || seenKeys.Contains(shortcut.GetId())) continue;
				ret.Add(shortcut);
				seenKeys.Add(shortcut.GetId());
			}
			
			return ret;
		}

		public void RefreshShortcuts(bool force)
		{
			new RefreshShortcutsTask(mContext, this).Execute(force);
		}

		private class RefreshShortcutsTask : AsyncTask<Java.Lang.Void, Java.Lang.Void, Java.Lang.Void>
		{
			private ShortcutHelper Helper { get; set; }
			private Context Context { get; set; }

			public RefreshShortcutsTask(Context context, ShortcutHelper helper)
			{
				Context = context;
				Helper = helper;
			}
			protected override Void RunInBackground(params Void[] @params)
			{
				Log.Info(TAG, "refreshingShortcuts...");

				var force = (bool) @params[0];

				var now = Java.Lang.JavaSystem.CurrentTimeMillis();
				var staleThreshold = force ? now : now - REFRESH_INTERVAL_MS;

				// Check all existing dynamic and pinned shortcut, and if their last refresh
				// time is older than a certain threshold, update them.

				List<ShortcutInfo> updateList = new List<ShortcutInfo>();

				foreach (var shortcut in updateList)
				{
					if (shortcut.IsImmutable())
					{
						continue;
					}

					PersistableBundle extras = shortcut.GetExtras();

					if (extras != null && extras.GetLong(EXTRA_LAST_REFRESH) >= staleThreshold)
					{
						// Shortcut still fresh.
						continue;
					}

					Log.Info(TAG, "Refreshing shortcut: " + shortcut.GetId());

					ShortcutInfo.Builder b = new ShortcutInfo.Builder(Context, shortcut.GetId());

					Helper.SetSiteInformation(b, shortcut.GetIntent().getData());

					Helper.SetExtras(b);

					updateList.Add(b.Build());
				}

				// Call update.
				if (updateList.Count > 0)
				{
					Helper.CallShortcutManager(()->mShortcutManager.updateShortcuts(updateList));
				}

				return null;
			}
		}

		private ShortcutInfo CreateShortcutForUrl(string urlAsString)
		{
			Log.Info(TAG, "createShortcutForUrl: " + urlAsString);

			ShortcutInfo.Builder b = new ShortcutInfo.Builder(mContext, urlAsString);

			var uri = Uri.Parse(urlAsString);
			b.SetIntent(new Intent(Intent.ActionView, uri));

			SetSiteInformation(b, uri);
			SetExtras(b);

			return b.Build();
		}

		private ShortcutInfo.Builder SetSiteInformation(ShortcutInfo.Builder b, Uri uri)
		{
			// TODO Get the actual site <title> and use it.
			// TODO Set the current locale to accept-language to get localized title.
			b.SetShortLabel(uri.Host);
			b.SetLongLabel(uri.ToString());

			var bmp = FetchFavicon(uri);
			b.SetIcon(bmp != null ? Icon.CreateWithBitmap(bmp) : Icon.CreateWithResource(mContext, Resource.Drawable.link));

			return b;
		}

		private ShortcutInfo.Builder SetExtras(ShortcutInfo.Builder b)
		{
			var extras = new PersistableBundle();
			extras.PutLong(EXTRA_LAST_REFRESH, Java.Lang.JavaSystem.CurrentTimeMillis());
			b.SetExtras(extras);
			return b;
		}

		private string NormalizeUrl(string urlAsString)
		{
			if (urlAsString.StartsWith("http://") || urlAsString.StartsWith("https://"))
			{
				return urlAsString;
			}
			return "http://" + urlAsString;
		}

		public void AddWebSiteShortcut(string urlAsString)
		{
			var uriFinal = urlAsString;
			CallShortcutManager(()-> {
				ShortcutInfo shortcut = CreateShortcutForUrl(NormalizeUrl(uriFinal));
				return mShortcutManager.AddDynamicShortcuts(Arrays.AsList(shortcut));
			});
		}

		public void RemoveShortcut(ShortcutInfo shortcut)
		{
			mShortcutManager.RemoveDynamicShortcuts(Arrays.AsList(shortcut.GetId()));
		}

		public void DisableShortcut(ShortcutInfo shortcut)
		{
			mShortcutManager.DisableShortcuts(Arrays.AsList(shortcut.GetId()));
		}

		public void EnableShortcut(ShortcutInfo shortcut)
		{
			mShortcutManager.EnableShortcuts(Arrays.AsList(shortcut.GetId()));
		}

		private Bitmap FetchFavicon(Uri uri)
		{
			var iconUri = uri.BuildUpon().Path("favicon.ico").Build();
			Log.Info(TAG, "Fetching favicon from: " + iconUri);

			BufferedStream bis = null;
			Stream inputStream;
			try
			{
				var conn = new URL(iconUri.ToString()).OpenConnection();
				conn.Connect();

				inputStream = conn.InputStream;
				bis = new BufferedStream(inputStream, 8192);
				return BitmapFactory.DecodeStream(bis);
			}
			catch (IOException e)
			{
				Log.Warn(TAG, "Failed to fetch favicon from " + iconUri, e);
				return null;
			}
		}
	}
}
