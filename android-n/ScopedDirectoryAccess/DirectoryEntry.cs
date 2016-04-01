using Android.OS;
using Java.Interop;

namespace ScopedDirectoryAccess
{
	/**
 	* Entity class that represents a directory entry.
 	*/
	public class DirectoryEntry : Java.Lang.Object, IParcelable
	{
		public string FileName { get; set;}
		public string MimeType { get; set;}

		public DirectoryEntry () { }

		public DirectoryEntry (Parcel inObj)
		{
			FileName = inObj.ReadString ();
			MimeType = inObj.ReadString ();
		}

		[ExportField ("CREATOR")]
		public static DirectoryEntryCreator InitializeCreator ()
		{
			return new DirectoryEntryCreator ();
		}

		public int DescribeContents ()
		{
			return 0;
		}

		public void WriteToParcel (Parcel parcel, ParcelableWriteFlags flags)
		{
			parcel.WriteString (FileName);
			parcel.WriteString (MimeType);
		}

		public class DirectoryEntryCreator : Java.Lang.Object, IParcelableCreator
		{
			public Java.Lang.Object CreateFromParcel (Parcel source)
			{
				return new DirectoryEntry (source);
			}

			public Java.Lang.Object [] NewArray (int size)
			{
				return new DirectoryEntry [size];
			}
		}
	}
}

