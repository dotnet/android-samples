using Android.App;
using Android.OS;

namespace com.xamarin.sample.fragments.honeycomb
{
    [Activity(Label = "Details Activity")]
    public class DetailsActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var index = Intent.Extras.GetInt("current_play_id", 0);

            var details = DetailsFragment.NewInstance(index); // Details
            var fragmentTransaction = FragmentManager.BeginTransaction();
            fragmentTransaction.Add(Android.Resource.Id.Content, details);
            fragmentTransaction.Commit();
        }
    }
}
