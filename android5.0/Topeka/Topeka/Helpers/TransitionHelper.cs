using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Util;
using Android.Views;

namespace Topeka.Helpers
{
	public static class TransitionHelper
	{
		public static Pair[] CreateSafeTransitionParticipants (Activity activity, bool includeStatusBar, params Pair[] otherParticipants)
		{
			var decor = activity.Window.DecorView;
			View statusBar = null;
			if (includeStatusBar)
				statusBar = decor.FindViewById (Android.Resource.Id.StatusBarBackground);
			var navBar = decor.FindViewById (Android.Resource.Id.NavigationBarBackground);

			var participants = new List<Pair> (3);
			AddNonNullViewToTransitionParticipants (statusBar, participants);
			AddNonNullViewToTransitionParticipants (navBar, participants);

			if (otherParticipants != null && !(otherParticipants.Length == 1 && otherParticipants [0] == null))
				participants.AddRange (otherParticipants.ToList ());

			return participants.ToArray ();
		}

		static void AddNonNullViewToTransitionParticipants (View view, List<Pair> participants)
		{
			if (view == null)
				return;

			participants.Add (new Pair (view, view.TransitionName));
		}
	}
}

