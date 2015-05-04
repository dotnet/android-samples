
using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Graphics;
using Supportv7Pallete.Utils;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Supportv7Pallete
{
	[Activity (Label = "Palette Example", ParentActivity=typeof(ImageListActivity))]			
	public class ImageDetailPaletteActivity : AppCompatActivity, Palette.IPaletteAsyncListener
	{
		PhotoItem item;
		TextView name;
		LinearLayout colors;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_image_detail);
			var toolbar = FindViewById<Toolbar> (Resource.Id.toolbar);

			SetSupportActionBar (toolbar);
		

			SupportActionBar.SetDisplayHomeAsUpEnabled (true);
			SupportActionBar.SetDisplayShowHomeEnabled (true);

			var id = Intent.GetIntExtra ("id", 0);
			item = Photos.GetPhoto (id);
			if (item == null)
				return;

			name = FindViewById<TextView> (Resource.Id.name);
			colors = FindViewById<LinearLayout> (Resource.Id.colors);
			var image = FindViewById<ImageView> (Resource.Id.image);

			image.SetImageResource (item.Image);
			name.Text =  item.Name;
			SupportActionBar.Title = item.Author;
			var paletteButton = FindViewById<Button> (Resource.Id.apply_palette);

			paletteButton.Click += async (sender, e) => {
				paletteButton.Visibility = ViewStates.Gone;

				//generates the pallet with 16 samples(default)
				//Contact images/avatars: optimal values are 24-32
				//Landscapes: optimal values are 8-16
				var bitmap = await BitmapFactory.DecodeResourceAsync (Resources, item.Image);
				Palette.From(bitmap)
					.MaximumColorCount(16)
					.Generate(this);
				
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
			if (palette.LightVibrantSwatch != null) {
				var lightVibrant = new Color (palette.LightVibrantSwatch.Rgb);
				name.SetBackgroundColor(lightVibrant);
				var color = new Color (lightVibrant);
				SupportActionBar.SetBackgroundDrawable (new ColorDrawable(color));
				if ((int)Build.VERSION.SdkInt >= 21)
					Window.SetStatusBarColor (color);
			}

			if (palette.DarkVibrantSwatch != null) {
				var darkVibrant = new Color (palette.DarkVibrantSwatch.Rgb);
				name.SetTextColor (darkVibrant);

				var view = FindViewById (Resource.Id.main_layout);
				view.SetBackgroundColor (darkVibrant);
			}

			var layoutParams = new LinearLayout.LayoutParams(30,30);
			//Loop through each of the palettes available
			//and put them as a small square
			foreach (var p in palette.Swatches) {
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

