using System;
using Android.Content;
using System.IO;
using Android.Util;
using System.Text;
using Org.Json;
using Android.Graphics;
using Java.IO;

namespace RecipeAssistant
{
	public static class AssetUtils
	{
		const string Tag = "RecipeAssistant";

		public static byte[] LoadAsset(Context context, String asset)
		{
			byte[] buffer;
			try 
			{
				using (Stream stream = context.Assets.Open(asset))
				{
					using (MemoryStream ms = new MemoryStream())
					{
						stream.CopyTo(ms);
						return ms.ToArray();
					}
				}
			}
			catch (Exception ex) {
				Log.Error (Tag, "Failed to load asset " + asset + ": " + ex);
				return null;
			}
		}

		public static JSONObject LoadJSONAsset (Context context, string asset)
		{
			string jsonString = Encoding.UTF8.GetString (LoadAsset (context, asset));
			try 
			{
				return new JSONObject(jsonString);
			}
			catch (Exception ex){
				Log.Error (Tag, "Failed to parse JSON asset " + asset + ": " + ex);
				return null;
			}
		}

		public static Bitmap LoadBitmapAsset(Context context, string asset)
		{
			try 
			{
				using (Stream stream = context.Assets.Open(asset))
				{
					if (stream != null)
					{
						return BitmapFactory.DecodeStream(stream);
					}
				}
			}
			catch (Exception ex) {
				Log.Error (Tag, ex.ToString ());
			}
			return null;
		}
	}
}

