using Android.App;

namespace MediaBrowserService
{
	[Activity (Label = "MediaBrowserService Sample", MainLauncher = true)]
	public class MusicPlayerActivity : Activity, BrowseFragment.IFragmentDataHelper
	{
		protected override void OnCreate (Android.OS.Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.activity_player);
			if (savedInstanceState == null) {
				FragmentManager.BeginTransaction ()
					.Add (Resource.Id.container, BrowseFragment.Create ())
					.Commit ();
			}
		}

		public void OnMediaItemSelected (Android.Media.Browse.MediaBrowser.MediaItem item)
		{
			if (item.IsPlayable) {
				MediaController.GetTransportControls ().PlayFromMediaId (item.MediaId, null);
				var queueFragment = QueueFragment.Create ();
				FragmentManager.BeginTransaction ()
					.Replace (Resource.Id.container, queueFragment)
					.AddToBackStack (null)
					.Commit ();
			} else if (item.IsBrowsable) {
				FragmentManager.BeginTransaction ()
					.Replace (Resource.Id.container, BrowseFragment.Create (item.MediaId))
					.AddToBackStack (null)
					.Commit ();
			}
		}
	}
}

