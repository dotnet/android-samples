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
	[Activity (Label = "GoogleIO2014Master", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainActivity : Activity
	{
		public static SparseArray<Bitmap> SPhotoCache = new SparseArray<Bitmap>(4);
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.activity_main);
		}
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.main, menu);
			return true;
		}

		[Export("showPhoto")]
		public void ShowPhoto(View view)
		{
			Intent intent = new Intent (this,typeof(DetailActivity));

			switch (view.Id) {
			case (Resource.Id.show_photo_1):
				intent.PutExtra ("lat", 37.6329946);
				intent.PutExtra ("lng", -122.49383444);
				intent.PutExtra ("zooom", 14.0f);
				intent.PutExtra("title", "Pacifica Pier");
				intent.PutExtra("description", this.Resources.GetText(Resource.String.lorem));
				intent.PutExtra("photo", Resource.Drawable.photo1);
				break;
			case (Resource.Id.show_photo_2):
				intent.PutExtra ("lat", 37.6329946);
				intent.PutExtra ("lng", -122.49383444);
				intent.PutExtra ("zooom", 14.0f);
				intent.PutExtra("title", "Pink Flamingo");
				intent.PutExtra("description", this.Resources.GetText(Resource.String.lorem));
				intent.PutExtra("photo", Resource.Drawable.photo2);
				break;
			case (Resource.Id.show_photo_3):
				intent.PutExtra ("lat", 37.6329946);
				intent.PutExtra ("lng", -122.49383444);
				intent.PutExtra ("zooom", 14.0f);
				intent.PutExtra("title", "Antelope Canyon");
				intent.PutExtra("description", this.Resources.GetText(Resource.String.lorem));
				intent.PutExtra("photo", Resource.Drawable.photo3);
				break;
			case (Resource.Id.show_photo_4):
				intent.PutExtra ("lat", 37.6329946);
				intent.PutExtra ("lng", -122.49383444);
				intent.PutExtra ("zooom", 14.0f);
				intent.PutExtra("title", "Line Pine");
				intent.PutExtra("description", this.Resources.GetText(Resource.String.lorem));
				intent.PutExtra("photo", Resource.Drawable.photo4);
				break;
			}

			var hero = ((View)(view.Parent)).FindViewById<ImageView> (Resource.Id.photo);
			SPhotoCache.Put(intent.GetIntExtra("photo", -1), ((BitmapDrawable)hero.Drawable).Bitmap);
			var options = ActivityOptions.MakeSceneTransitionAnimation (this, hero, "photo_hero");
			StartActivity (intent,options.ToBundle());
		}
	}
}


