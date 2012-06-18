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
using Android.Content.PM;
using Uri = Android.Net.Uri;

namespace MonoIO
{
	/**
	 * Helper class for the Catch Notes integration, based on example code at
	 * {@link https://github.com/catch/docs-api/}.
	 */
	public class CatchNotesHelper
	{
		private static String TAG = "CatchNotesHelper";
	
	    // Intent actions
	    public static String ACTION_ADD = "com.catchnotes.intent.action.ADD";
	    public static String ACTION_VIEW = "com.catchnotes.intent.action.VIEW";
	
	    // Intent extras for ACTION_ADD
	    public static String EXTRA_SOURCE = "com.catchnotes.intent.extra.SOURCE";
	    public static String EXTRA_SOURCE_URL = "com.catchnotes.intent.extra.SOURCE_URL";
	
	    // Intent extras for ACTION_VIEW
	    public static String EXTRA_VIEW_FILTER = "com.catchnotes.intent.extra.VIEW_FILTER";
	
	    // Note: "3banana" was the original name of Catch Notes. Though it has been
	    // rebranded, the package name must persist.
	    private static String NOTES_PACKAGE_NAME = "com.threebanana.notes";
	    private static String NOTES_MARKET_URI = "http://market.android.com/details?id=" + NOTES_PACKAGE_NAME;
	
	    private static int NOTES_MIN_VERSION_CODE = 54;
	
	    private Context mContext;
	
	    public CatchNotesHelper(Context context) 
		{
	        mContext = context;
	    }
	
	    public Intent CreateNoteIntent(String message) {
	        if (!IsNotesInstalledAndMinimumVersion()) {
	            return NotesMarketIntent();
	        }
	
	        Intent intent = new Intent();
	        intent.SetAction(ACTION_ADD);
	        intent.PutExtra(Intent.ExtraText, message);
	        intent.PutExtra(EXTRA_SOURCE, mContext.GetString(Resource.String.app_name));
	        intent.PutExtra(EXTRA_SOURCE_URL, "http://www.google.com/io/");
	        intent.PutExtra(Intent.ExtraTitle, mContext.GetString(Resource.String.app_name));
	        return intent;
	    }
	
	    public Intent ViewNotesIntent(String tag) {
	        if (!IsNotesInstalledAndMinimumVersion()) {
	            return NotesMarketIntent();
	        }
	
	        if (!tag.StartsWith("#")) {
	            tag = "#" + tag;
	        }
	
	        Intent intent = new Intent();
	        intent.SetAction(ACTION_VIEW);
	        intent.PutExtra(EXTRA_VIEW_FILTER, tag);
	        intent.SetFlags(ActivityFlags.NewTask);
	        return intent;
	    }
	
	    /**
	     * Returns the installation status of Catch Notes.
	     */
	    public bool IsNotesInstalledAndMinimumVersion() {
	        try {
	            var packageInfo = mContext.PackageManager.GetPackageInfo(NOTES_PACKAGE_NAME, PackageInfoFlags.Activities);
	            if (packageInfo.VersionCode < NOTES_MIN_VERSION_CODE) {
	                return false;
	            }
	        } catch (Exception e) {
	            return false;
	        }
	
	        return true;
	    }
	
	    public Intent NotesMarketIntent() {
	        Uri uri = Uri.Parse(NOTES_MARKET_URI);
	        Intent intent = new Intent(Intent.ActionView, uri);
	        intent.SetFlags(ActivityFlags.ClearWhenTaskReset);
	        return intent;
	    }
	}
}

