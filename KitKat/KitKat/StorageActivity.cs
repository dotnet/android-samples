using System;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System.IO;
using System.Runtime.CompilerServices;
using Android.Provider;
using Android.Graphics;
using Android.InputMethodServices;
using System.Threading.Tasks;

namespace KitKat
{
	[Activity(Label = "StorageActivity")]			
	public class StorageActivity : Activity
	{
		const int read_request_code = 0;
		const int write_request_code = 1;
		Button loadPhotoButton;
		Button createDocButton;
		ImageView imageView;
		Android.Net.Uri currentImageUri;
		ImageViewHelper imageViewHelper;

		protected override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
			if (currentImageUri == null)
			{
				outState.PutString("current_image_uri", string.Empty);
			}
			else
			{
				outState.PutString("current_image_uri", currentImageUri.ToString());
			}
		}

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			imageViewHelper = new ImageViewHelper(this);

			SetContentView(Resource.Layout.Storage);

			loadPhotoButton = FindViewById<Button>(Resource.Id.loadPhotoButton);
			createDocButton = FindViewById<Button>(Resource.Id.createDocButton);

			if (bundle != null)
			{
				currentImageUri = Android.Net.Uri.Parse(bundle.GetString("current_image_uri", string.Empty));
			}
			else
			{
				currentImageUri = null;
			}

			// This button lets a user pick a photo from the Storage Access Framework UI
			loadPhotoButton.Click += (o, e) =>
			{

				// I want data from all available providers!
				Intent intentOpen = new Intent(Intent.ActionOpenDocument);

				// filter for files that can be opened/used
				intentOpen.AddCategory(Intent.CategoryOpenable);

				//filter results by mime type, if applicable
				intentOpen.SetType("image/*");
				StartActivityForResult(intentOpen, read_request_code);

			};

			// This button takes a photo and saves it to the Storage Access Framework UI
			// The user will be asked what they want to name the file, 
			// and what directory they want to save it in

			createDocButton.Click += (o, e) =>
			{
				Intent intentCreate = new Intent(Intent.ActionCreateDocument);
				intentCreate.AddCategory(Intent.CategoryOpenable);

				// We're going to add a text document
				intentCreate.SetType("text/plain");

				// Pass in a default name for the new document - 
				// the user will be able to change this
				intentCreate.PutExtra(Intent.ExtraTitle, "NewDoc");
				StartActivityForResult(intentCreate, write_request_code);
			};
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			if (resultCode == Result.Ok && data != null && requestCode == read_request_code)
			{
				currentImageUri = data.Data;
			}
			else
			{
				currentImageUri = null;
			}

			if (resultCode == Result.Ok && data != null && requestCode == write_request_code)
			{
				// This returns a URI for the newly created file, which we will then write to
				WriteToSaf(data.Data);
			}
		}

		async Task DisplayImageAsync(Android.Net.Uri imageUri)
		{
			imageView = FindViewById<ImageView>(Resource.Id.imageView);
			currentImageUri = imageUri;
			await imageViewHelper.DisplayPictureAsync(imageView, imageUri);
		}

		public override async void OnWindowFocusChanged(bool hasFocus)
		{
			// We display the image in this method because here we know that 
			// the ImageView has been inflated and attached to the window activity.
			base.OnWindowFocusChanged(hasFocus);
			if ((currentImageUri != null) && (hasFocus))
			{
				await DisplayImageAsync(currentImageUri);
			}

		}

		void WriteToSaf(Android.Net.Uri uri)
		{

			// Create a stream using the URI passed to us on activity result.
			// Note that ContentResolver.OpenOutputStream(Android.Net.Uri) 
			// returns a System.IO.Stream

			using (Stream stream = ContentResolver.OpenOutputStream(uri))
			{

				// We'll encode a string and write it to the doc
				Encoding u8 = Encoding.UTF8;
				string content = "Hello, world!";
				stream.Write(u8.GetBytes(content), 0, content.Length);

				// The file will be written to and saved in the directory 
				// specified by the user
			}


		}
	}
}
