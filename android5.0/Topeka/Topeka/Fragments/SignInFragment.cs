using System;

using Android.App;
using Android.OS;
using Android.Support.V4.View.Animation;
using Android.Util;
using Android.Views;
using Android.Widget;

using Topeka.Activities;
using Topeka.Adapters;
using Topeka.Helpers;
using Topeka.Widgets.Fab;

namespace Topeka.Fragments
{
	public class SignInFragment : Fragment
	{
		const string ArgEdit = "EDIT";
		const string KeySelectedAvatarIndex = "selectedAvatarIndex";

		Player player;
		EditText firstName;
		EditText lastInitial;
		Avatar selectedAvatar = Avatar.One;
		View selectedAvatarView;
		GridView avatarGrid;
		DoneFab doneFab;
		bool edit;

		public static SignInFragment Create (bool edit)
		{
			var args = new Bundle ();
			args.PutBoolean (ArgEdit, edit);
			var fragment = new SignInFragment ();
			fragment.Arguments = args;
			return fragment;
		}

		public override void OnCreate (Android.OS.Bundle savedInstanceState)
		{
			if (savedInstanceState != null) {
				var savedAvatarIndex = savedInstanceState.GetInt (KeySelectedAvatarIndex);
				selectedAvatar = (Avatar)savedAvatarIndex;
			}
			base.OnCreate (savedInstanceState);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View contentView = inflater.Inflate (Resource.Layout.fragment_sign_in, container, false);
			EventHandler<View.LayoutChangeEventArgs> handler = null;
			handler = ((sender, e) => {
				((View)sender).LayoutChange -= handler;
				SetUpGridView (View);
			});
			contentView.LayoutChange += handler;
			return contentView;
		}

		public override void OnSaveInstanceState (Bundle outState)
		{
			outState.PutInt (KeySelectedAvatarIndex, selectedAvatar.Ordinal ());
			base.OnSaveInstanceState (outState);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			AssurePlayerInit ();
			CheckIsInEditMode ();

			if (player == null || edit) {
				view.FindViewById (Resource.Id.empty).Visibility = ViewStates.Gone;
				view.FindViewById (Resource.Id.content).Visibility = ViewStates.Visible;
				InitContentViews (view);
				InitContents ();
			} else {
				var activity = Activity;
				CategorySelectionActivity.Start (activity, player);
				activity.Finish ();
			}
			base.OnViewCreated (view, savedInstanceState);
		}

		void CheckIsInEditMode ()
		{
			edit = Arguments != null && Arguments.GetBoolean (ArgEdit, false);
		}

		void InitContentViews (View view)
		{
			firstName = view.FindViewById<EditText> (Resource.Id.first_name);
			firstName.TextChanged += (sender, e) => doneFab.Visibility = string.Concat (e.Text).Length == 0 ? ViewStates.Gone : ViewStates.Visible;
			lastInitial = view.FindViewById<EditText> (Resource.Id.last_initial);
			lastInitial.TextChanged += (sender, e) => doneFab.Visibility = string.Concat (e.Text).Length == 0 ? ViewStates.Gone : ViewStates.Visible;
			doneFab = view.FindViewById<DoneFab> (Resource.Id.done);
			doneFab.Click += (sender, e) => {
				var v = (View)sender;
				switch (v.Id) {
				case Resource.Id.done:
					SavePlayer (Activity);
					var runnable = new Runnable ();
					runnable.RunAction += (s, ea) => {
						if (selectedAvatarView == null)
							PerformSignInWithTransition (avatarGrid.GetChildAt (selectedAvatar.Ordinal ()));
						else
							PerformSignInWithTransition (selectedAvatarView);
					};
					RemoveDoneFab (runnable);
					break;
				default:
					throw new InvalidOperationException ("The onClick method has not been implemented for " + Resources.GetResourceEntryName (v.Id));
				}
			};
		}

		void RemoveDoneFab (Runnable endAction)
		{
			doneFab.Animate ()
				.ScaleX (0)
				.ScaleY (0)
				.SetInterpolator (new FastOutSlowInInterpolator ())
				.WithEndAction (endAction)
				.Start ();
		}

		void SetUpGridView (View container)
		{
			avatarGrid = container.FindViewById<GridView> (Resource.Id.avatars);
			avatarGrid.Adapter = new AvatarAdapter (Activity);
			avatarGrid.ItemClick += (sender, e) => {
				selectedAvatarView = e.View;
				selectedAvatar = (Avatar)Enum.GetValues (typeof(Avatar)).GetValue (e.Position);
			};
			avatarGrid.NumColumns = CalculateSpanCount ();
			avatarGrid.SetItemChecked (selectedAvatar.Ordinal (), true);
		}


		void PerformSignInWithTransition (View v)
		{
			var activity = Activity;

			var pairs = TransitionHelper.CreateSafeTransitionParticipants (activity, true,
				            new Pair (v, activity.GetString (Resource.String.transition_avatar)));
			var activityOptions = ActivityOptions.MakeSceneTransitionAnimation (activity, pairs);
			CategorySelectionActivity.Start (activity, player, activityOptions);
		}

		void InitContents ()
		{
			AssurePlayerInit ();
			if (null != player) {
				firstName.Text = player.FirstName;
				lastInitial.Text = player.LastInitial;
				selectedAvatar = player.Avatar;
			}
		}

		void AssurePlayerInit ()
		{
			if (null == player)
				player = PreferencesHelper.GetPlayer (Activity);
		}

		void SavePlayer (Activity activity)
		{
			player = new Player (firstName.Text, lastInitial.Text, selectedAvatar);
			PreferencesHelper.WriteToPreferences (activity, player);
		}

		int CalculateSpanCount ()
		{
			var avatarSize = Resources.GetDimensionPixelSize (Resource.Dimension.size_fab);
			var avatarPadding = Resources.GetDimensionPixelSize (Resource.Dimension.spacing_double);
			return avatarGrid.Width / (avatarSize + avatarPadding);
		}
	}
}

