using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace com.xamarin.sample.fragments.honeycomb
{
    public class TitlesFragment : ListFragment
    {
        private int _currentPlayId;
        private bool _isDualPane;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            var detailsFrame = Activity.FindViewById<View>(Resource.Id.details);
            _isDualPane = detailsFrame != null && detailsFrame.Visibility == ViewStates.Visible;

            var adapter = new ArrayAdapter<String>(Activity, Android.Resource.Layout.SimpleListItemChecked, Shakespeare.Titles);
            ListAdapter = adapter;

            if (savedInstanceState != null)
            {
                _currentPlayId = savedInstanceState.GetInt("current_play_id", 0);
            }

            if (_isDualPane)
            {
                ListView.ChoiceMode = ChoiceMode.Single;
                ShowDetails(_currentPlayId);
            }
        }

        public override void OnListItemClick(ListView l, View v, int position, long id)
        {
            ShowDetails(position);
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutInt("current_play_id", _currentPlayId);
        }

        private void ShowDetails(int playId)
        {
            _currentPlayId = playId;
            if (_isDualPane)
            {
                // We can display everything in-place with fragments.
                // Have the list highlight this item and show the data.
                ListView.SetItemChecked(playId, true);

                // Check what fragment is shown, replace if needed.
                var details = FragmentManager.FindFragmentById(Resource.Id.details) as DetailsFragment;
                if (details == null || details.ShownPlayId != playId)
                {
                    // Make new fragment to show this selection.
                    details = DetailsFragment.NewInstance(playId);

                    // Execute a transaction, replacing any existing
                    // fragment with this one inside the frame.
                    var ft = FragmentManager.BeginTransaction();
                    ft.Replace(Resource.Id.details, details);
                    ft.SetTransition(FragmentTransit.FragmentFade);
                    ft.Commit();
                }
            }
            else
            {
                // Otherwise we need to launch a new activity to display
                // the dialog fragment with selected text.
                var intent = new Intent();

                intent.SetClass(Activity, typeof(DetailsActivity));
                intent.PutExtra("current_play_id", playId);
                StartActivity(intent);
            }
        }
    }
}
