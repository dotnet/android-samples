namespace ViewPagerIndicator_Example
{
    using Android.App;
    using Android.OS;
    using Android.Support.V4.App;
    using Android.Support.V4.View;

    [Activity(Label = "ViewPagerIndicator Example", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : FragmentActivity
    {
        public TestFragmentAdapter _fragmentAdapter;
        public ViewPagerIndicator.IPageIndicator _pageIndicator;
        public ViewPager _viewPager;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            _fragmentAdapter = new TestFragmentAdapter(SupportFragmentManager);
            _viewPager = FindViewById<ViewPager>(Resource.Id.pager);
            _viewPager.Adapter = _fragmentAdapter;

            _pageIndicator = FindViewById<ViewPagerIndicator.TitlePageIndicator>(Resource.Id.indicator);
            _pageIndicator.SetViewPager(_viewPager);
        }
    }
}
