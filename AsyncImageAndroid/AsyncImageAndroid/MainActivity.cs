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
		int count = 1;
		WebClient webClient;
		Button downloadButton;
		Button clickButton;
		TextView infoLabel;
		ImageView imageview;
		ProgressBar downloadProgress;

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

			downloadButton.Click += downloadAsync;

			clickButton.Click += delegate {
				clickButton.Text = string.Format ("{0} clicks!", count++);
			};
		}

		async void downloadAsync(object sender, System.EventArgs ea)
		{
			var manager = new DownloadManager ();
			SetDownloading ();

			string localPath = string.Empty;
			try {
				localPath = await manager.StartDownload();
			} catch(TaskCanceledException){
				Console.WriteLine ("Task Canceled!");
				return;
			} catch(Exception e) {
				Console.WriteLine (e.ToString());
				SetReadyToDownload ();
				return;
			}

			infoLabel.Text = "Resizing Image...";

			var bitmap = await FetchBitmap (localPath);
			imageview.SetImageBitmap (bitmap);
			SetReadyToDownload ();
		}

		void SetReadyToDownload ()
		{
			infoLabel.Text = "Click Dowload button to download the image";

			downloadButton.Click -= cancelDownload;
			downloadButton.Click += downloadAsync;
			downloadButton.Text = "Download";

			downloadProgress.Progress = 0;
		}

		void SetDownloading ()
		{
			downloadButton.Text = "Cancel";
			downloadButton.Click -= downloadAsync;
			downloadButton.Click += cancelDownload;

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

		void cancelDownload(object sender, System.EventArgs ea)
		{
			Console.WriteLine ("Cancel clicked!");
			if(webClient!=null)
				webClient.CancelAsync ();

			webClient.DownloadProgressChanged -= HandleDownloadProgressChanged;
			SetReadyToDownload ();
		}
	}
}


