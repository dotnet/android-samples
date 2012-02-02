//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//
//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using Android.Support.V4.App;
//using Android.Content.PM;
//using Java.IO;
//using Android.Graphics.Drawables;
//using Android.Content.Res;
//using System.Threading.Tasks;
//
//namespace Support4
//{
//	[Activity (Label = "@string/loader_custom_support")]
//	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.support4demo.sample" })]
//	public class LoaderCustomSupport : FragmentActivity
//	{
//		protected override void OnCreate (Bundle bundle)
//		{
//			base.OnCreate (bundle);
//			
//			var fm = SupportFragmentManager;
//
//	        // Create the list fragment and add it as our sole content.
//	        if (fm.FindFragmentById(Android.Resource.Id.Content) == null) {
//	            var list = new AppListFragment();
//	            fm.BeginTransaction().Add(Android.Resource.Id.Content, list).Commit();
//	        }
//		}
//		
//		/**
//	     * This class holds the per-item data in our Loader.
//	     */
//	    public class AppEntry 
//		{
//			
//	        private AppListLoader _loader;
//	        private ApplicationInfo _info;
//	        private File _apkFile;
//	        private string _label;
//	        private Drawable _icon;
//	        private bool _mounted;
//			
//	        public AppEntry(AppListLoader loader, ApplicationInfo info) 
//			{
//	            _loader = loader;
//	            _info = info;
//	            _apkFile = new File(info.SourceDir);
//	        }
//	
//	        public ApplicationInfo GetApplicationInfo() {
//	            return _info;
//	        }
//	
//	        public String GetLabel() {
//	            return _label;
//	        }
//	
//	        public Drawable GetIcon() {
//	            if (_icon == null) {
//	                if (_apkFile.Exists()) 
//					{
//	                    _icon = _info.LoadIcon(_loader.Pm);
//	                    return _icon;
//	                } else {
//	                    _mounted = false;
//	                }
//	            } else if (!_mounted) {
//	                // If the app wasn't mounted but is now mounted, reload
//	                // its icon.
//	                if (_apkFile.Exists()) {
//	                    _mounted = true;
//	                    _icon = _info.LoadIcon(_loader.Pm);
//	                    return _icon;
//	                }
//	            } else {
//	                return _icon;
//	            }
//	
//	            return mLoader.getContext().getResources().getDrawable(
//	                    android.R.drawable.sym_def_app_icon);
//	        }
//	
//			public override string ToString ()
//			{
//				return _label;
//			}
//	
//	        public void LoadLabel(Context context) {
//	            if (_label == null || !_mounted) {
//	                if (!_apkFile.Exists()) {
//	                    _mounted = false;
//	                    _label = _info.PackageName;
//	                } else {
//	                    _mounted = true;
//	                    var label = _info.LoadLabel(context.PackageManager);
//	                    _label = label != null ? label.ToString() : _info.PackageName;
//	                }
//	            }
//	        }
//	    }
//		
//		/**
//	     * Perform alphabetical comparison of application entry objects.
//	     */
////	    public static final Comparator<AppEntry> ALPHA_COMPARATOR = new Comparator<AppEntry>() {
////	        private final Collator sCollator = Collator.getInstance();
////	        @Override
////	        public int compare(AppEntry object1, AppEntry object2) {
////	            return sCollator.compare(object1.getLabel(), object2.getLabel());
////	        }
////	    };
//
//	    /**
//	     * Helper for determining if the configuration has changed in an interesting
//	     * way so we need to rebuild the app list.
//	     */
////	    public class InterestingConfigChanges 
////		{
////	        Configuration lastConfiguration = new Configuration();
////	        int _lastDensity;
////	
////			boolean applyNewConfig(Resources res) {
////	            int configChanges = mLastConfiguration.updateFrom(res.getConfiguration());
////	            boolean densityChanged = mLastDensity != res.getDisplayMetrics().densityDpi;
////	            if (densityChanged || (configChanges&(ActivityInfo.CONFIG_LOCALE
////	                    |ActivityInfoCompat.CONFIG_UI_MODE|ActivityInfo.CONFIG_SCREEN_LAYOUT)) != 0) {
////	                mLastDensity = res.getDisplayMetrics().densityDpi;
////	                return true;
////	            }
////	            return false;
////	        }
////	    }
//		
//		/**
//	     * Helper class to look for interesting changes to the installed apps
//	     * so that the loader can be updated.
//	     */
//	    public class PackageIntentReceiver : BroadcastReceiver 
//		{
//	        AppListLoader _loader;
//	
//	        public PackageIntentReceiver(AppListLoader loader) {
//	            _loader = loader;
//	            var filter = new IntentFilter(Intent.ActionPackageAdded);
//	            filter.AddAction(Intent.ActionPackageRemoved);
//	            filter.AddAction(Intent.ActionPackageChanged);
//	            filter.AddDataScheme("package");
//	            _loader.Context.RegisterReceiver(this, filter);
//	            // Register for events related to sdcard installation.
//	            IntentFilter sdFilter = new IntentFilter();
//	            sdFilter.AddAction(Intent.ActionExternalApplicationsAvailable);
//	            sdFilter.AddAction(Intent.ActionExternalApplicationsUnavailable);
//	            _loader.Context.RegisterReceiver(this, sdFilter);
//	        }
//	
//			public override void OnReceive (Context context, Intent intent)
//			{
//				_loader.OnContentChanged();
//			}
//	    }
//		
//		public class AppListLoader
//		{
//			//final InterestingConfigChanges mLastConfig = new InterestingConfigChanges();
//	        public PackageManager Pm { get; set; }
//	
//	        List<AppEntry> _apps;
//	        PackageIntentReceiver _packageObserver;
//			
//			
//			
//			public AppListLoader(Context context) 
//			{
//	            // Retrieve the package manager for later use; note we don't
//	            // use 'context' directly but instead the save global application
//	            // context returned by getContext().
//	            Pm = context.PackageManager;
//				
//				Task.Factory.StartNew(() => {
//					
//					// Retrieve all known applications.
//					// How do I add flags? such as PackageInfoFlags.UninstalledPackages or PackageInfoFlags.DisabledComponents
//	            	var apps = Pm.GetInstalledApplications((int)PackageInfoFlags.DisabledComponents);
//					
//		            if (apps == null) {
//		                apps = new List<ApplicationInfo>();
//		            }
//		
//		
//		            // Create corresponding array of entries and load their labels.
//		            List<AppEntry> entries = new List<AppEntry>(apps.Count);
//					foreach( var app in apps)
//					{
//						var entry = new AppEntry(this, app);
//						entry.LoadLabel(context);
//						entries.Add (entry);
//					}
//		
//					
//		            // Sort the list.
//		            //Collections.sort(entries, ALPHA_COMPARATOR);
//		
//					
//		            // Done!
//					_apps = entries;
//					
//				}).ContinueWith(task => {
//						
//					
//					
//				});
//
//	        }
//		}
//	}
//}
//
