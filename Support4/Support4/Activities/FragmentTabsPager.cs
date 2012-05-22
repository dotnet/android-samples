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
using Android.Support.V4.App;
using Java.Lang;
using Android.Support.V4.View;

namespace Support4
{
	[Activity (Label = "@string/fragment_tabs_pager")]
	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.support4demo.sample" })]
	public class FragmentTabsPager : FragmentActivity
	{
		
		TabHost tabHost;
		ViewPager  viewPager;
    	TabsAdapter tabsAdapter;
		
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			
			SetContentView(Resource.Layout.fragment_tabs_pager);
	        tabHost = FindViewById<TabHost>(Android.Resource.Id.TabHost);
	        tabHost.Setup();
			
			viewPager = FindViewById<ViewPager>(Resource.Id.pager);

        	tabsAdapter = new TabsAdapter(this, tabHost, viewPager);

	
	        tabsAdapter.AddTab(tabHost.NewTabSpec("simple").SetIndicator("Simple"), Java.Lang.Class.FromType(typeof(FragmentStackSupport.CountingFragment)), null);
			tabsAdapter.AddTab(tabHost.NewTabSpec("contacts").SetIndicator("Custom"), Java.Lang.Class.FromType(typeof(LoaderCursorSupport.CursorLoaderListFragment)), null);				
				
	        if (savedInstanceState != null) {
	            tabHost.SetCurrentTabByTag(savedInstanceState.GetString("tab"));
	        }
		}
		
		protected override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
			
			outState.PutString("tab", tabHost.CurrentTabTag);

		}
		
		
	    /**
	     * This is a helper class that implements the management of tabs and all
	     * details of connecting a ViewPager with associated TabHost.  It relies on a
	     * trick.  Normally a tab host has a simple API for supplying a View or
	     * Intent that each tab will show.  This is not sufficient for switching
	     * between pages.  So instead we make the content part of the tab host
	     * 0dp high (it is not shown) and the TabsAdapter supplies its own dummy
	     * view to show as the tab content.  It listens to changes in tabs, and takes
	     * care of switch to the correct paged in the ViewPager whenever the selected
	     * tab changes.
	     */
		protected class TabsAdapter : FragmentPagerAdapter, TabHost.IOnTabChangeListener, ViewPager.IOnPageChangeListener
		{
			private Context _context;
	        private TabHost _tabHost;
	        private ViewPager _viewPager;
	        private List<TabInfo> _tabs = new List<TabInfo>();
			
			public class TabInfo 
			{
	            public string tag;
	            public Class clss;
	            public Bundle args;
	            public Fragment fragment {get; set;}
	
	            public TabInfo(string _tag, Class _class, Bundle _args) {
	                tag = _tag;
	                clss = _class;
	                args = _args;
	            }
	        }
			
			public class DummyTabFactory : Java.Lang.Object, TabHost.ITabContentFactory 
			{
	            private Context _context;
	
	            public DummyTabFactory(Context context) {
	                _context = context;
	            }
	
				public View CreateTabContent (string tag)
				{
					var v = new View(_context);
					v.SetMinimumHeight(0);
					v.SetMinimumWidth(0);
					return v;
				}
	        }
			
			public TabsAdapter(FragmentActivity activity, TabHost tabHost, ViewPager pager) : base(activity.SupportFragmentManager)
			{
	            _context = activity;
	            _tabHost = tabHost;
	            _viewPager = pager;
	            _tabHost.SetOnTabChangedListener(this);
				_viewPager.Adapter = this;
	            _viewPager.SetOnPageChangeListener(this);
				
	        }
			
			public void AddTab(TabHost.TabSpec tabSpec, Class clss, Bundle args) 
			{
	            tabSpec.SetContent(new DummyTabFactory(_context));
	            var tag = tabSpec.Tag;
	
	            var info = new TabInfo(tag, clss, args);
	
	            _tabs.Add(info);
	            _tabHost.AddTab(tabSpec);
				NotifyDataSetChanged();
	        }
			
			public override int Count {
				get {
					return _tabs.Count;
				}
			}
			
			public override Fragment GetItem (int position)
			{
				var info = _tabs[position];
            	return Fragment.Instantiate(_context, info.clss.Name, info.args);
			}
			
			public void OnTabChanged (string tabId)
			{
				int position = _tabHost.CurrentTab;
            	_viewPager.CurrentItem = position;
			}
			
			public void OnPageScrolled (int position, float positionOffset, int positionOffsetPixels)
			{
				
			}
			
			public void OnPageSelected (int position)
			{
				// Unfortunately when TabHost changes the current tab, it kindly
            	// also takes care of putting focus on it when not in touch mode.
	            // The jerk.
	            // This hack tries to prevent this from pulling focus out of our
	            // ViewPager.
	            var widget = _tabHost.TabWidget;
	            var oldFocusability = widget.DescendantFocusability;
	            widget.DescendantFocusability = DescendantFocusability.BlockDescendants;
	            _tabHost.CurrentTab = position;
	            widget.DescendantFocusability = oldFocusability;
			}
			
			public void OnPageScrollStateChanged (int state)
			{
				
			}
			
		}
	}
}

