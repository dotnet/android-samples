using System.Collections.Generic;
using Android.Provider;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace ScopedDirectoryAccess
{
	/**
 	* Provide views to RecyclerView with the directory entries.
 	*/
	public class DirectoryEntryAdapter : RecyclerView.Adapter
	{
		public List<DirectoryEntry> DirectoryEntries { get; set; }

		public DirectoryEntryAdapter () 
			: this (new List<DirectoryEntry> ())
		{
		}

		public DirectoryEntryAdapter (List<DirectoryEntry> directoryEntries)
		{
			DirectoryEntries = directoryEntries;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder (Android.Views.ViewGroup parent, int viewType)
		{
			View v = LayoutInflater.From (parent.Context).Inflate (Resource.Layout.directory_entry, parent, false);
			return new ViewHolder (v);
		}

		public override void OnBindViewHolder (RecyclerView.ViewHolder holder, int position)
		{
			((ViewHolder)holder).FileName.Text = DirectoryEntries[position].FileName;
			((ViewHolder)holder).MimeType.Text = DirectoryEntries[position].MimeType;

			if (DocumentsContract.Document.MimeTypeDir == DirectoryEntries[position].MimeType)
				((ViewHolder)holder).ImageView.SetImageResource (Resource.Drawable.ic_directory_grey600_36dp);
			else
				((ViewHolder)holder).ImageView.SetImageResource (Resource.Drawable.ic_description_grey600_36dp);
		}

		public override int ItemCount {
			get {
				return DirectoryEntries.Count;
			}
		}
	}

	public class ViewHolder : RecyclerView.ViewHolder
	{
		public TextView FileName { get; set; }
		public TextView MimeType { get; set; }
		public ImageView ImageView { get; set; }

		public ViewHolder (View v) : base (v)
		{
			FileName = (TextView)v.FindViewById (Resource.Id.textview_filename);
			MimeType = (TextView)v.FindViewById (Resource.Id.textview_mimetype);
			ImageView = (ImageView)v.FindViewById (Resource.Id.imageview_entry);
		}
	}
}

