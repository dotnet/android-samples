using System;

using Android.OS;

namespace Topeka.Helpers
{
	public class Creator<T> : Java.Lang.Object, IParcelableCreator where T : Java.Lang.Object
	{
		public EventHandler<CreatorEventArgs> Created { get; set; }

		public virtual Java.Lang.Object CreateFromParcel (Parcel source)
		{
			Java.Lang.Object result = null;
			if (Created != null) {
				var args = new CreatorEventArgs (source);
				Created (this, args);
				result = args.Result;
			}
			return result;
		}

		public virtual Java.Lang.Object[] NewArray (int size)
		{
			return new T[size];
		}
	}

	public class CreatorEventArgs : EventArgs
	{
		public Parcel Source { get; set; }

		public Java.Lang.Object Result { get; set; }

		public CreatorEventArgs (Parcel source)
		{
			Source = source;
		}
	}
}