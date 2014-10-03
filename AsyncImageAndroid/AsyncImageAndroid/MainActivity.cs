using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;

using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace AsyncImageAndroid
{

	[Activity (Label = "AsyncImageAndroid", MainLauncher = true,
	           ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation)]
	public class MainActivity : Activity
	{
		const string downloadUrl = "http://photojournal.jpl.nasa.gov/jpeg/PIA15416.jpg";

		int count = 1;
		Button downloadButton;
		Button clickButton;
		TextView infoLabel;
		ImageView imageview;
		ProgressBar downloadProgress;

		DownloadManager manager;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			this.downloadButton = FindViewById<Button> (Resource.Id.downloadButton);
			this.clickButton = FindViewById<Button> (Resource.Id.clickButton);
			this.infoLabel = FindViewById<TextView> (Resource.Id.textView1);
			this.imageview = FindViewById<ImageView> (Resource.Id.imageView1);
			this.downloadProgress = FindViewById<ProgressBar> (Resource.Id.progressBar);

			downloadButton.Click += DownloadAsync;

			clickButton.Click += delegate {
				clickButton.Text = string.Format ("{0} clicks!", count++);
			};
		}

		async void DownloadAsync(object sender, EventArgs args)
		{
			var manager = new DownloadManager ();
			// We can leave this unsubscribed because manager is shortlive object and Target of event (this) is longlive
			manager.DownloadProgressChanged += HandleDownloadProgressChanged;
			// Here is a problem. clickButton is longlive object and manager is shortlive.
			// This means that we will prolong livetime of manager, because we reference to manager will be stored implicitly 
			clickButton.Click += manager.EventHandler;

			SetDownloading ();

			string filePath = string.Empty;
			try {
				filePath = await manager.DownloadAsync(downloadUrl, "downloaded.png");
			} catch(TaskCanceledException) {
				Console.WriteLine ("Task Canceled!");
				SetReadyToDownload ();
				return;
			} catch(Exception exc) {
				Console.WriteLine (exc);
				SetReadyToDownload ();
				return;
			}

			infoLabel.Text = "Resizing Image...";

			var bitmap = await FetchBitmap (filePath);
			imageview.SetImageBitmap (bitmap);
			SetReadyToDownload ();

			// Solution is here
//			clickButton.Click -= manager.EventHandler;
		}

		void SetReadyToDownload ()
		{
			infoLabel.Text = "Click Dowload button to download the image";

			downloadButton.Click -= CancelDownload;
			downloadButton.Click += DownloadAsync;
			downloadButton.Text = "Download";

			downloadProgress.Progress = 0;
		}

		void SetDownloading ()
		{
			downloadButton.Text = "Cancel";
			downloadButton.Click -= DownloadAsync;
			downloadButton.Click += CancelDownload;

			infoLabel.Text = "Downloading...";
		}

		async Task<Bitmap> FetchBitmap(string imagePath)
		{
			BitmapFactory.Options options = new BitmapFactory.Options ();
			options.InJustDecodeBounds = true;
			await BitmapFactory.DecodeFileAsync (imagePath, options);

			options.InSampleSize = options.OutWidth > options.OutHeight ? options.OutHeight / imageview.Height : options.OutWidth / imageview.Width;
			options.InJustDecodeBounds = false;

			Bitmap bitmap = await BitmapFactory.DecodeFileAsync (imagePath, options);
			return bitmap;
		}

		void HandleDownloadProgressChanged (object sender, DownloadProgressChangedEventArgs e)
		{
			this.downloadProgress.Progress = e.ProgressPercentage;
		}

		void CancelDownload(object sender, System.EventArgs ea)
		{
			Console.WriteLine ("Cancel clicked!");
			SetReadyToDownload ();
		}
	}
}


