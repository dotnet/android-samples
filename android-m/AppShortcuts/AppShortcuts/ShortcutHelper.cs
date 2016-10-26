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
using Android.Content;

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
			mShortcutManager = mContext.GetSystemService(ShortcutManager.class));
    	}
	
		public void MaybeRestoreAllDynamicShortcuts()
		{
			if (mShortcutManager.getDynamicShortcuts().size() == 0)
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
				if (!r.getAsBoolean())
				{
					Utils.ShowToast(mContext, "Call to ShortcutManager is rate-limited");
				}
			}
			catch (Exception e)
			{
				Log.e(TAG, "Caught Exception", e);
				Utils.ShowToast(mContext, "Error while calling ShortcutManager: " + e.toString());
			}
		}

		/**
	     * Return all mutable shortcuts from this app self.
	     */
	    public List<ShortcutInfo> GetShortcuts()
		{
			// Load mutable dynamic shortcuts and pinned shortcuts and put them into a single list
			// removing duplicates.

			final List< ShortcutInfo> ret = new ArrayList<ShortcutInfo>();
			final HashSet< String > seenKeys = new HashSet<>();

			// Check existing shortcuts shortcuts
			for (ShortcutInfo shortcut : mShortcutManager.GetDynamicShortcuts())
			{
				if (!shortcut.isImmutable())
				{
					ret.add(shortcut);
					seenKeys.add(shortcut.getId());
				}
			}
			for (ShortcutInfo shortcut : mShortcutManager.getPinnedShortcuts())
			{
				if (!shortcut.isImmutable() && !seenKeys.contains(shortcut.getId()))
				{
					ret.add(shortcut);
					seenKeys.add(shortcut.getId());
				}
			}
			return ret;
		}
	}
}
