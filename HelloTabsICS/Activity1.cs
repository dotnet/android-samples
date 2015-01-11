using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace HelloTabsICS
{
    [Activity (Label = "HelloTabsICS", MainLauncher = true)]
    public class Activity1 : Activity
    {   
        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
   
            this.ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
                 
            AddTab ("Tab 1", Resource.Drawable.ic_tab_white, new SampleTabFragment ());
            AddTab ("Tab 2", Resource.Drawable.ic_tab_white, new SampleTabFragment2 ());

            if (bundle != null)
                this.ActionBar.SelectTab(this.ActionBar.GetTabAt(bundle.GetInt("tab")));

        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutInt("tab", this.ActionBar.SelectedNavigationIndex);

            base.OnSaveInstanceState(outState);
        }
        
        void AddTab (string tabText, int iconResourceId, Fragment view)
        {
            var tab = this.ActionBar.NewTab ();            
            tab.SetText (tabText);
            tab.SetIcon (Resource.Drawable.ic_tab_white);
            
            // must set event handler before adding tab
            tab.TabSelected += delegate(object sender, ActionBar.TabEventArgs e)
            {
                var fragment = this.FragmentManager.FindFragmentById(Resource.Id.fragmentContainer);
                if (fragment != null)
                    e.FragmentTransaction.Remove(fragment);         
                e.FragmentTransaction.Add (Resource.Id.fragmentContainer, view);
            };
            tab.TabUnselected += delegate(object sender, ActionBar.TabEventArgs e) {
                e.FragmentTransaction.Remove(view);
            };
            
            this.ActionBar.AddTab (tab);
        }
        
        class SampleTabFragment: Fragment
        {            
            public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                base.OnCreateView (inflater, container, savedInstanceState);
                
                var view = inflater.Inflate (Resource.Layout.Tab, container, false);
                var sampleTextView = view.FindViewById<TextView> (Resource.Id.sampleTextView);             
                sampleTextView.Text = "sample fragment text";

                return view;
            }
        }

        class SampleTabFragment2 : Fragment
        {
            public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                base.OnCreateView(inflater, container, savedInstanceState);

                var view = inflater.Inflate(Resource.Layout.Tab, container, false);
                var sampleTextView = view.FindViewById<TextView>(Resource.Id.sampleTextView);
                sampleTextView.Text = "sample fragment text 2";

                return view;
            }
        }
    }
}


