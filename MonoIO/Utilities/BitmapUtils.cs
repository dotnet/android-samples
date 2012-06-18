using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Java.IO;
using Java.Security;
using Environment = Android.OS.Environment;
using Android.Util;
using System.Threading.Tasks;
using Android.Text;

namespace MonoIO
{
	/**
	 * Helper class for fetching and disk-caching images from the web.
	 */
	public class BitmapUtils 
	{
	    private static String TAG = "BitmapUtils";
		private static Bitmap result;
		
	    /**
	     * Only call this method from the main (UI) thread. The {@link OnFetchCompleteListener} callback
	     * be invoked on the UI thread, but image fetching will be done in an {@link AsyncTask}.
	     */
	    public static void FetchImage(Context context, Java.Lang.String url, Action<Bitmap> callback) {
	        FetchImage(context, url, null, null, callback);
	    }
	
	    /**
	     * Only call this method from the main (UI) thread. The {@link OnFetchCompleteListener} callback
	     * be invoked on the UI thread, but image fetching will be done in an {@link AsyncTask}.
	     *
	     * @param cookie An arbitrary object that will be passed to the callback.
	     */
	    public static void FetchImage(Context context, Java.Lang.String url, BitmapFactory.Options decodeOptions, Java.Lang.Object cookie, Action<Bitmap> callback) 
		{
			
			Task.Factory.StartNew(() => {
				if(TextUtils.IsEmpty(url))
				{
					result = null;
					return;
				}
				
				File cacheFile = null;
				try
				{
					MessageDigest mDigest = MessageDigest.GetInstance("SHA-1");
					mDigest.Update(url.GetBytes());
					string cacheKey = BytesToHexString(mDigest.Digest());
					if(Environment.MediaMounted.Equals(Environment.ExternalStorageState))
					{
						cacheFile = new File(Environment.ExternalStorageDirectory +
						                     File.Separator + "Android " + 
						                     File.Separator + "data" +
						                     File.Separator + context.PackageName + 
						                     File.Separator + "cache" +
						                     File.Separator + "bitmap_" + cacheKey + ".tmp");
					}
				}
				catch (Exception e) 
				{
					// NoSuchAlgorithmException
					// Oh well, SHA-1 not available (weird), don't cache bitmaps.
				}
				
				if (cacheFile != null && cacheFile.Exists()) 
				{
	                Bitmap cachedBitmap = BitmapFactory.DecodeFile(cacheFile.ToString(), decodeOptions);
                    if (cachedBitmap != null) {
                        result = cachedBitmap;
						return;
                    }
                }
				
				try 
				{
				    // TODO: check for HTTP caching headers
					var client = new System.Net.WebClient();
					var image = client.DownloadData(new Uri(url.ToString()));
					if (image != null)
					{
						result = null;
						return;
					}
				
				    // Write response bytes to cache.
				    if (cacheFile != null) {
				        try {
				            cacheFile.ParentFile.Mkdirs();
				            cacheFile.CreateNewFile();
				            FileOutputStream fos = new FileOutputStream(cacheFile);
				            fos.Write(image);
				            fos.Close();
				        } catch (FileNotFoundException e) {
				            Log.Warn(TAG, "Error writing to bitmap cache: " + cacheFile.ToString(), e);
				        } catch (IOException e) {
				            Log.Warn(TAG, "Error writing to bitmap cache: " + cacheFile.ToString(), e);
				        }
				    }
				
				    // Decode the bytes and return the bitmap.
				    result = (BitmapFactory.DecodeByteArray(image, 0, image.Length, decodeOptions));
					return;
				} catch (Exception e) {
				    Log.Warn(TAG, "Problem while loading image: " + e.ToString(), e);
				}
                result = null;
			})
			.ContinueWith(task =>
				callback(result)
        	);
			
	    }
	
	    private static String BytesToHexString(byte[] bytes) {
	        // http://stackoverflow.com/questions/332079
	        Java.Lang.StringBuffer sb = new Java.Lang.StringBuffer();
	        for (int i = 0; i < bytes.Length; i++) {
	            String hex = Java.Lang.Integer.ToHexString(0xFF & bytes[i]);
	            if (hex.Length == 1) {
	                sb.Append('0');
	            }
	            sb.Append(hex);
	        }
	        return sb.ToString();
	    }
	}
}

