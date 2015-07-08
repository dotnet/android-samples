using System;
using Android.Graphics;

namespace MediaBrowserService
{
	public class LruCache : Android.Util.LruCache 
	{

		public Func<string, Bitmap[], int> GetSizeOf;

		public LruCache(int maxSize) : base(maxSize) {}

		protected override int SizeOf (Java.Lang.Object key, Java.Lang.Object value)
		{
			return GetSizeOf(key.ToString(), (Bitmap[])value);
		}

		public Bitmap[] this[string key]
		{
			get {
				return (Bitmap[])Get (key);	
			}
		}
	}
}

