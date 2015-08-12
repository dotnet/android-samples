using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Graphics;
using Android.Graphics.Drawables;
using Java.Interop;

namespace GoogleIO2014Master
{
	[Activity (Label = "@string/app_name", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainActivity : Activity
	{
		public static SparseArray<Bitmap> SPhotoCache = new SparseArray<Bitmap>(4);

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.activity_main);
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.main, menu);
			return true;
		}

		[Export ("showPhoto")]
		public void ShowPhoto (View view)
		{
			Intent intent = new Intent (this, typeof (DetailActivity));

			switch (view.Id) {
			case (Resource.Id.show_photo_1):
				intent.PutExtra ("lat", 37.6329946);
				intent.PutExtra ("lng", -122.4938344);
				intent.PutExtra ("zooom", 14.0f);
				intent.PutExtra ("title", "Pacifica Pier");
				intent.PutExtra ("description", Resources.GetText (Resource.String.lorem));
				intent.PutExtra ("photo", Resource.Drawable.photo1);
				break;
			case (Resource.Id.show_photo_2):
				intent.PutExtra ("lat", 37.73284);
				intent.PutExtra ("lng", -122.503065);
				intent.PutExtra ("zoom", 15.0f);
				intent.PutExtra ("title", "Pink Flamingo");
				intent.PutExtra ("description", Resources.GetText (Resource.String.lorem));
				intent.PutExtra ("photo", Resource.Drawable.photo2);
				break;
			case (Resource.Id.show_photo_3):
				intent.PutExtra ("lat", 36.861897);
				intent.PutExtra ("lng", -111.374438);
				intent.PutExtra ("zoom", 11.0f);
				intent.PutExtra ("title", "Antelope Canyon");
				intent.PutExtra ("description", Resources.GetText (Resource.String.lorem));
				intent.PutExtra ("photo", Resource.Drawable.photo3);
				break;
			case (Resource.Id.show_photo_4):
				intent.PutExtra ("lat", 36.596125);
				intent.PutExtra ("lng", -118.1604282);
				intent.PutExtra ("zoom", 9.0f);
				intent.PutExtra ("title", "Lone Pine");
				intent.PutExtra ("description", Resources.GetText (Resource.String.lorem));
				intent.PutExtra ("photo", Resource.Drawable.photo4);
				break;
			}

			var hero = ((View)(view.Parent)).FindViewById<ImageView> (Resource.Id.photo);
			SPhotoCache.Put (intent.GetIntExtra ("photo", -1), ((BitmapDrawable)hero.Drawable).Bitmap);
			ActivityOptions options = ActivityOptions.MakeSceneTransitionAnimation (this, hero, "photo_hero");
			StartActivity (intent,options.ToBundle());
		}
	}
}

