
using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Graphics;
using AndroidLSamples.Utils;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace AndroidLSamples
{
	[Activity (Label = "Palette Example", ParentActivity=typeof(HomeActivity))]			
	public class ImageDetailPaletteActivity : Activity, Palette.IPaletteAsyncListener
	{
		PhotoItem item;
		TextView name;
		LinearLayout colors;
		protected async override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_image_detail);
			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetDisplayShowHomeEnabled (true);
			ActionBar.SetIcon (Android.Resource.Color.Transparent);

			var id = Intent.GetIntExtra ("id", 0);
			item = Photos.GetPhoto (id);
			if (item == null)
				return;

			name = FindViewById<TextView> (Resource.Id.name);
			colors = FindViewById<LinearLayout> (Resource.Id.colors);
			var image = FindViewById<ImageView> (Resource.Id.image);

			image.SetImageResource (item.Image);
			name.Text = ActionBar.Title = item.Name;

			FindViewById<Button> (Resource.Id.apply_palette).Click += async (sender, e) => {

				var bitmap = await BitmapFactory.DecodeResourceAsync (Resources, item.Image);

				//generates the pallet with 16 samples(default)
				//Contact images/avatars: optimal values are 24-32
				//Landscapes: optimal values are 8-16
				Palette.GenerateAsync (bitmap, 16, this);
			};

		}

		public void OnGenerated (Palette palette)
		{
			//Pallet has been generated with 6 coors to pick from:
			//Vibrant: palette.VibrantColor
			//Vibrant dark: palette.DarkVibrantColor
			//Vibrant light: palette.LightVibrantColor
			//Muted: palette.MutedColor
			//Muted dark: palette.DarkMutedColor
			//Muted light: palette.LightMutedColor
			if (palette == null)
				return;
				

			//must check each palette as there is no guarantee
			//that it was generated.
			if (palette.LightVibrantColor != null) {
				var lightVibrant = new Color (palette.LightVibrantColor.Rgb);
				name.SetBackgroundColor(lightVibrant);
				ActionBar.SetBackgroundDrawable (new ColorDrawable (new Color(lightVibrant)));
			}

			if (palette.DarkVibrantColor != null) {
				var darkVibrant = new Color (palette.DarkVibrantColor.Rgb);

				var actionBarTitleId = Android.Content.Res.Resources.System.GetIdentifier ("action_bar_title", "id", "android");
				if (actionBarTitleId > 0) {
					var title = FindViewById<TextView> (actionBarTitleId);
					if (title != null)
						title.SetTextColor (darkVibrant);
				}

				var view = FindViewById (Resource.Id.main_layout);
				view.SetBackgroundColor (darkVibrant);
			}

			var layoutParams = new LinearLayout.LayoutParams(30,30);
			//Loop through each of the palettes available
			//and put them as a small square
			foreach (var p in palette.Pallete) {
				if (p == null)
					continue;

				var view = new View (this);
				view.SetBackgroundColor (new Color (p.Rgb));
				view.LayoutParameters = layoutParams;
				colors.AddView (view, 0);
			}
		}
	}
}

