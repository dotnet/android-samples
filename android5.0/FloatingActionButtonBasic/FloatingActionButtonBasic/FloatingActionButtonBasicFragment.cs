using Android.App;
using Android.Net;
using Android.OS;
using Android.Views;

using CommonSampleLibrary;

namespace FloatingActionButtonBasic
{
	// This fragment inflates a layout with two Floating Action Buttons and acts as a listener to changes on them.
	public class FloatingActionButtonBasicFragment : Fragment, FloatingActionButton.IOnCheckedChangeListener
	{
		private const string TAG = "FloatingActionButtonBasicFragment";
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Inflate the layout for this fragment.
			View rootView = inflater.Inflate (Resource.Layout.fab_layout, container, false);

			// Make this Fragment listen for changes in both FABs.
			var fab1 = (FloatingActionButton)rootView.FindViewById (Resource.Id.fab_1);
			fab1.SetOnCheckedChangeListener (this);
			var fab2 = (FloatingActionButton)rootView.FindViewById (Resource.Id.fab_2);
			fab2.SetOnCheckedChangeListener (this);
			return rootView;

		}

		public void OnCheckedChanged(FloatingActionButton fabView, bool isChecked)
		{
			// When a FAB is toggled, log the action.
			switch (fabView.Id) {
			case Resource.Id.fab_1: 
				Log.Debug (TAG, string.Format ("FAB 1 was {0}.", isChecked ? "checked" : "unchecked"));
				break;
			case Resource.Id.fab_2:
				Log.Debug (TAG, string.Format ("FAB 2 was {0}.", isChecked ? "checked" : "unchecked"));
				break;
			default:
				break;
			}

		}
	}
}

