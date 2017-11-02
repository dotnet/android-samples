using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Util;

namespace com.xamarin.example.actionbar.tabs.support
{
	[Android.App.Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat", MainLauncher = true, Icon = "@drawable/ic_launcher")]
    public class MainActivity : AppCompatActivity, ActionBar.ITabListener
    {
        static readonly string Tag = "ActionBarTabsSupport";
        Fragment[] _fragments;

        public void OnTabReselected(ActionBar.Tab tab, FragmentTransaction ft)
        {
            Log.Debug(Tag, "The tab {0} was re-selected.", tab.Text);
        }

        public void OnTabSelected(ActionBar.Tab tab, FragmentTransaction ft)
        {
            Log.Debug(Tag, "The tab {0} has been selected.", tab.Text);
            Fragment frag = _fragments[tab.Position];
            ft.Replace(Resource.Id.frameLayout1, frag);
        }

        public void OnTabUnselected(ActionBar.Tab tab, FragmentTransaction ft)
        {
            // perform any extra work associated with saving fragment state here.
            Log.Debug(Tag, "The tab {0} as been unselected.", tab.Text);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            _fragments = new Fragment[]
                         {
                             new WhatsOnFragment(),
                             new SpeakersFragment(),
                             new SessionsFragment()
                         };

            AddTabToActionBar(Resource.String.whatson_tab_label, Resource.Drawable.ic_action_whats_on);
            AddTabToActionBar(Resource.String.speakers_tab_label, Resource.Drawable.ic_action_speakers);
            AddTabToActionBar(Resource.String.sessions_tab_label, Resource.Drawable.ic_action_sessions);
        }

        void AddTabToActionBar(int labelResourceId, int iconResourceId)
        {
            ActionBar.Tab tab = SupportActionBar.NewTab()
                                                .SetText(labelResourceId)
                                                .SetIcon(iconResourceId)
                                                .SetTabListener(this);
            SupportActionBar.AddTab(tab);
        }
    }
}
