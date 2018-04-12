using Android.App;
using Android.OS;

namespace FragmentSample
{
    [Activity(Label = "PlayQuoteActivity")]
    public class PlayQuoteActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var playId = Intent.Extras.GetInt("current_play_id", 0);

            var detailsFrag = PlayQuoteFragment.NewInstance(playId);
            FragmentManager.BeginTransaction()
                           .Add(Android.Resource.Id.Content, detailsFrag)
                           .Commit();
        }
    }
}
