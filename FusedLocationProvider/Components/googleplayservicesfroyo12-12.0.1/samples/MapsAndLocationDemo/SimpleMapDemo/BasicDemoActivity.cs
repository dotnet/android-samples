namespace SimpleMapDemo
{
    using Android.App;
    using Android.OS;
	using Android.Support.V4.App;

    [Activity(Label = "@string/basic_map")]
    public class BasicDemoActivity : FragmentActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.BasicDemo);
        }
    }
}
