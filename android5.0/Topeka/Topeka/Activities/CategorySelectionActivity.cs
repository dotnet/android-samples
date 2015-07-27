using System;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Topeka.Fragments;
using Topeka.Helpers;

using Topeka.Persistence;

namespace Topeka.Activities
{
	[Activity (Theme = "@style/Topeka.CategorySelectionActivity")]
	public class CategorySelectionActivity : Activity
	{
		const string ExtraPlayer = "player";

		public static void Start (Context context, Player player, ActivityOptions options)
		{
			var starter = new Intent (context, typeof(CategorySelectionActivity));
			starter.PutExtra (ExtraPlayer, player);
			context.StartActivity (starter, options.ToBundle ());
		}

		public static void Start (Context context, Player player)
		{
			var starter = new Intent (context, typeof(CategorySelectionActivity));
			starter.PutExtra (ExtraPlayer, player);
			context.StartActivity (starter);
		}

		protected override void OnCreate (Android.OS.Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.activity_category_selection);
			var player = (Player)Intent.GetParcelableExtra (ExtraPlayer);
			SetUpToolbar (player);
			if (savedInstanceState == null)
				AttachCategoryGridFragment ();
			else
				SetProgressBarVisibility (ViewStates.Gone);
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			var scoreView = FindViewById<TextView> (Resource.Id.score);
			var score = TopekaDatabaseHelper.GetScore (this);
			scoreView.Text = string.Format (GetString (Resource.String.x_points), score);
		}

		void SetUpToolbar (Player player)
		{
			var toolbar = FindViewById<Toolbar> (Resource.Id.toolbar_player);
			SetActionBar (toolbar);

			ActionBar.SetDisplayShowTitleEnabled (false);
			var avatarView = toolbar.FindViewById<AvatarView> (Resource.Id.avatar);
			avatarView.SetImageDrawable (GetDrawable ((int)player.Avatar));
			(toolbar.FindViewById<TextView> (Resource.Id.title)).Text = GetDisplayName (player);
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.menu_category, menu);
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (item.ItemId == Resource.Id.sign_out) {
				SignOut ();
				return true;
			}
	
			return base.OnOptionsItemSelected (item);
		}

		void SignOut ()
		{
			PreferencesHelper.SignOut (this);
			TopekaDatabaseHelper.Reset (this);
			SignInActivity.Start (this, false, null);
			FinishAfterTransition ();
		}

		string GetDisplayName (Player player)
		{
			return string.Format (GetString (Resource.String.player_display_name), player.FirstName, player.LastInitial);
		}

		void AttachCategoryGridFragment ()
		{
			FragmentManager.BeginTransaction ().Replace (Resource.Id.quiz_container, CategorySelectionFragment.Create ()).Commit ();
			SetProgressBarVisibility (ViewStates.Gone);
		}

		void SetProgressBarVisibility (ViewStates visibility)
		{
			FindViewById (Resource.Id.progress).Visibility = visibility;
		}
	}
}

