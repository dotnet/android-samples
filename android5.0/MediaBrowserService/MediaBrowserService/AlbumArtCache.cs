using System;
using Android.Graphics;
using System.Threading.Tasks;
using System.IO;

namespace MediaBrowserService
{
	public sealed class AlbumArtCache
	{
		static readonly string Tag = LogHelper.MakeLogTag (typeof(AlbumArtCache));

		const int MaxAlbumArtCacheSize = 12 * 1024 * 1024;
		const int MaxArtWidth = 800;
		const int MaxArtHeight = 480;

		const int MaxArtWidthIcon = 128;
		const int MaxArtHeightIcon = 128;

		const int BigBitmapIndex = 0;
		const int IconBitmapIndex = 1;

		readonly LruCache cache;

		static AlbumArtCache instance;

		public static AlbumArtCache Instance {
			get {
				if (instance == null)
					instance = new AlbumArtCache ();
				return instance;
			}
		}

		AlbumArtCache() {
			var maxSize = Math.Min(MaxAlbumArtCacheSize,
				(int) (Math.Min(int.MaxValue, Java.Lang.Runtime.GetRuntime().MaxMemory() / 4)));
			cache = new LruCache (maxSize);
			cache.GetSizeOf = (key, value) => value [BigBitmapIndex].ByteCount + value [IconBitmapIndex].ByteCount;
		}

		public Bitmap GetBigImage(string artUrl) {
			var result = cache [artUrl];
			return result == null ? null : result[BigBitmapIndex];
		}

		public Bitmap GetIconImage(string artUrl) {
			var result = cache [artUrl];
			return result == null ? null : result[IconBitmapIndex];
		}

		public void Fetch(string artUrl, FetchListener listener) {
			// WARNING: for the sake of simplicity, simultaneous multi-thread fetch requests
			// are not handled properly: they may cause redundant costly operations, like HTTP
			// requests and bitmap rescales. For production-level apps, we recommend you use
			// a proper image loading library, like Glide.
			var bMap = cache [artUrl];
			if (bMap != null) {
				LogHelper.Debug(Tag, "getOrFetch: album art is in cache, using it", artUrl);
				listener.OnFetched(artUrl, bMap[BigBitmapIndex], bMap[IconBitmapIndex]);
				return;
			}
			LogHelper.Debug(Tag, "getOrFetch: starting asynctask to fetch ", artUrl);

			Task.Run (async () => {
				Bitmap[] bitmaps;
				try {
					var bitmap = await BitmapHelper.FetchAndRescaleBitmap(artUrl, MaxArtWidth, MaxArtHeight);
					var icon = BitmapHelper.Scale(bitmap, MaxArtWidthIcon, MaxArtHeightIcon);
					bitmaps = new [] {bitmap, icon};
					cache.Put(artUrl, bitmaps);
				} catch(IOException) {
					return null;
				}
				LogHelper.Debug(Tag, "doInBackground: putting bitmap in cache. cache size=" + cache.Size());
				return bitmaps;
			}).ContinueWith ((antecedent) => {
				var bitmaps = antecedent.Result;
				if (bitmaps == null)
					listener.OnError(artUrl, new ArgumentException("got null bitmaps"));
				else
					listener.OnFetched(artUrl, bitmaps[BigBitmapIndex], bitmaps[IconBitmapIndex]);
			}, TaskContinuationOptions.OnlyOnRanToCompletion);
		}

		public class FetchListener {
			public Action<string, Bitmap, Bitmap> OnFetched;
			public void OnError(string artUrl, Exception e) {
				LogHelper.Error(Tag, e, "AlbumArtFetchListener: error while downloading " + artUrl);
			}
		}
	}
}

