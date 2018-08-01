using Android.App;
using Android.OS;
using Android.Support.V7.App;

namespace SimpleMapDemo
{
    [Activity(Label = "@string/activity_label_axml")]
    public class BasicDemoActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.BasicDemo);
        }
    }
}
