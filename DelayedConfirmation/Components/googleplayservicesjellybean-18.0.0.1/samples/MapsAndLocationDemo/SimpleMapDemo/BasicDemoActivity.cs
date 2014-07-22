namespace SimpleMapDemo
{
    using Android.App;
    using Android.OS;

    [Activity(Label = "@string/basic_map")]
    public class BasicDemoActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.BasicDemo);
        }
    }
}
