using System;

using Android.App;
using Android.Animation;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V7.Graphics;
using Android.Transitions;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Android.Gms.Common;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;

using Java.Lang;
using Java.Interop;

using GoogleIO2014Master.UI;


namespace GoogleIO2014Master
{
	[Activity (ParentActivity = typeof(MainActivity), Theme = "@style/DetailTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class DetailActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.activity_detail);

			Bitmap photo = SetupPhoto (Intent.GetIntExtra ("photo", Resource.Drawable.photo1));

			Colorize (photo);

			SetupMap ();
			SetupText ();

			SetOutlines (Resource.Id.star, Resource.Id.info);
			ApplySystemWindowsBottomInsert (Resource.Id.container);

			Window.EnterTransition.AddListener (new DetailsTransitionAdapter (this));
		}

		public override void OnBackPressed ()
		{
			var hero = FindViewById<ImageView> (Resource.Id.photo);
			var color = ObjectAnimator.OfArgb (hero.Drawable, "tint", Resources.GetColor (Resource.Color.photo_tint), 0);
			color.AnimationEnd += delegate {
				FinishAfterTransition ();
			};
			color.Start ();
			FindViewById (Resource.Id.info).Animate ().Alpha (0.0f);
			FindViewById (Resource.Id.star).Animate ().Alpha (0.0f);
		}

		public void SetupText ()
		{
			var titleView = FindViewById<TextView> (Resource.Id.title);
			titleView.SetText (Intent.GetStringExtra ("title"), TextView.BufferType.Normal);

			var descriptionView = FindViewById<TextView> (Resource.Id.description);
			descriptionView.SetText (Intent.GetStringExtra ("description"), TextView.BufferType.Normal);
		}

		public void SetupMap ()
		{
			GoogleMap map = (FragmentManager.FindFragmentById<MapFragment> (Resource.Id.map)).Map;

			double lat = Intent.GetDoubleExtra ("lat", 37.6329946);
			double lng = Intent.GetDoubleExtra ("lng", -122.4938344);
			float zoom = Intent.GetFloatExtra ("zoom", 15.0f);

			var position = new LatLng (lat, lng);
			map.MoveCamera (CameraUpdateFactory.NewLatLngZoom (position, zoom));
			map.AddMarker (new MarkerOptions ().SetPosition (position));
		}

		public void SetOutlines (int star, int info)
		{
			var vop = new MyVop (this);

			FindViewById (star).OutlineProvider = vop;
			FindViewById (info).OutlineProvider = vop;
		}

		class MyVop : ViewOutlineProvider
		{
			DetailActivity da;

			int size;

			public MyVop (DetailActivity det)
			{
				da = det;
				size = da.Resources.GetDimensionPixelSize (Resource.Dimension.floating_button_size);
			}

			public override void GetOutline (View view, Outline outline)
			{
				outline.SetOval (0, 0, size, size);
			}
		}

		public void ApplySystemWindowsBottomInsert (int container)
		{
			View containerView = FindViewById (container);
			containerView.SetFitsSystemWindows (true);
			containerView.ApplyWindowInsets = delegate (View v, WindowInsets insets) {
				DisplayMetrics metrics = Resources.DisplayMetrics;

				if (metrics.WidthPixels < metrics.HeightPixels)
					v.SetPadding (0, 0, 0, insets.SystemWindowInsetBottom);
				else
					v.SetPadding (0, 0, insets.SystemWindowInsetRight, 0);
				
				return insets.ConsumeSystemWindowInsets ();
			};
		}

		public void Colorize (Bitmap bitmap)
		{
			Palette palette = new Palette.Builder (bitmap).Generate ();
			ApplyPalette (palette);
		}

		void ApplyPalette (Palette palette)
		{
			// Null check, as swatch value might be null.
			if (palette.DarkMutedSwatch != null)
				Window.SetBackgroundDrawable (new ColorDrawable (new Color (palette.DarkMutedSwatch.Rgb)));

			var star = FindViewById<AnimatedPathView> (Resource.Id.star_container);
			var titleView = FindViewById<TextView> (Resource.Id.title);
			if (palette.VibrantSwatch != null) {
				star.FillColor = palette.VibrantSwatch.Rgb;
				titleView.SetTextColor (new Color (palette.VibrantSwatch.Rgb));
			}

			var descriptionView = FindViewById<TextView> (Resource.Id.description);
			if (palette.LightVibrantSwatch != null) {
				star.StrokeColor = palette.LightVibrantSwatch.Rgb;
				descriptionView.SetTextColor (new Color (palette.LightVibrantSwatch.Rgb));
			}

			if (palette.DarkMutedSwatch != null && palette.DarkVibrantSwatch != null)
				ColorRipple (Resource.Id.info, palette.DarkMutedSwatch.Rgb, palette.DarkVibrantSwatch.Rgb);

			if (palette.MutedSwatch != null && palette.VibrantSwatch != null)
				ColorRipple (Resource.Id.star, palette.MutedSwatch.Rgb, palette.VibrantSwatch.Rgb);

			var infoView = FindViewById (Resource.Id.information_container);
			if (palette.LightMutedSwatch != null)
				infoView.SetBackgroundColor (new Color (palette.LightMutedSwatch.Rgb));
		}

		public void ColorRipple (int id, int bgColor, int tintColor)
		{
			var buttonView = FindViewById (id);

			var ripple = (RippleDrawable)(buttonView.Background);
			var rippleBackground = (GradientDrawable)(ripple.GetDrawable (0));
			rippleBackground.SetColor(bgColor);

			ripple.SetColor (ColorStateList.ValueOf (new Color (tintColor)));
		}

		public Bitmap SetupPhoto (int resource)
		{
			var bitmap = MainActivity.SPhotoCache.Get (resource);
			FindViewById<ImageView> (Resource.Id.photo).SetImageBitmap (bitmap);
			return bitmap;
		}

		[Export ("showStar")]
		public void ShowStar (View view)
		{
			ToggleStarView ();
		}

		public void ToggleStarView ()
		{
			var starContainer = FindViewById<AnimatedPathView> (Resource.Id.star_container);

			if (starContainer.Visibility == ViewStates.Invisible) {
				FindViewById (Resource.Id.photo).Animate ().Alpha (0.2f);
				starContainer.Alpha = 1.0f;
				starContainer.Visibility = ViewStates.Visible;
				starContainer.Reveal ();
			} else {
				FindViewById (Resource.Id.photo).Animate ().Alpha (1.0f);
				starContainer.Animate ().Alpha (0.0f).WithEndAction (new Runnable (new Action (delegate {
					starContainer.Visibility = ViewStates.Invisible;
				})));
			}
		}

		[Export ("showInformation")]
		public void ShowInformation (View view)
		{
			ToggleInformationView (view);
		}

		public void ToggleInformationView (View view)
		{
			View infoContainer = FindViewById (Resource.Id.information_container);
			int cx = (view.Left + view.Right) / 2;
			int cy = (view.Top + view.Bottom) / 2;

			float radius = System.Math.Max (infoContainer.Width, infoContainer.Height) * 2.0f;

			Animator reveal;
			if (infoContainer.Visibility == ViewStates.Invisible) {
				infoContainer.Visibility = ViewStates.Visible;
				reveal = ViewAnimationUtils.CreateCircularReveal (infoContainer, cx, cy, 0, radius);
				reveal.SetInterpolator (new AccelerateInterpolator (2.0f));
			} else {
				reveal = ViewAnimationUtils.CreateCircularReveal (infoContainer, cx, cy, 0, radius);
				reveal.AnimationEnd += delegate {
					infoContainer.Visibility = ViewStates.Invisible;
				};
				reveal.SetInterpolator (new DecelerateInterpolator (2.0f));
			}
			reveal.SetDuration (600);
			reveal.Start ();
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.detail, menu);
			return true;
		}

		public class DetailsTransitionAdapter : TransitionAdapter
		{
			public DetailActivity da;

			public DetailsTransitionAdapter (DetailActivity details)
			{
				da = details;
			}

			public override void OnTransitionEnd (Transition transition)
			{
				var hero = da.FindViewById<ImageView> (Resource.Id.photo);
				ObjectAnimator color = ObjectAnimator.OfArgb (hero.Drawable, "tint",
					                       da.Resources.GetColor (Resource.Color.photo_tint), 0);
				color.Start ();

				da.FindViewById (Resource.Id.info).Animate ().Alpha (1.0f);
				da.FindViewById (Resource.Id.star).Animate ().Alpha (1.0f);
				da.Window.EnterTransition.RemoveListener (this);
			}
		}
	}
}

