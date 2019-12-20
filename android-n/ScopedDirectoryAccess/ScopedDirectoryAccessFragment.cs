using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.OS.Storage;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content;
using Android.Net;
using Android.Database;
using AndroidX.RecyclerView.Widget;

namespace ScopedDirectoryAccess
{
	/**
 	* Fragment that displays the directory contents.
 	*/
	public class ScopedDirectoryAccessFragment : Fragment
	{
		static readonly string DIRECTORY_ENTRIES_KEY = "directory_entries";
		static readonly string SELECTED_DIRECTORY_KEY = "selected_directory";
		static readonly int OPEN_DIRECTORY_REQUEST_CODE = 1;

		static readonly string[] DIRECTORY_SELECTION = {
			DocumentsContract.Document.ColumnDisplayName,
			DocumentsContract.Document.ColumnMimeType,
			DocumentsContract.Document.ColumnDocumentId
    	};

		StorageManager storageManager;
		TextView currentDirectoryTextView;
		TextView nothingInDirectoryTextView;
		Spinner directoriesSpinner;
		DirectoryEntryAdapter adapter;
		List<DirectoryEntry> directoryEntries;

		public static ScopedDirectoryAccessFragment NewInstance ()
		{
			return new ScopedDirectoryAccessFragment ();
		}

		public override void OnAttach (Context context)
		{
			base.OnAttach (context);

			storageManager = Activity.GetSystemService (Java.Lang.Class.FromType (typeof (StorageManager))).JavaCast<StorageManager> ();
		}

		public override void OnActivityResult (int requestCode, Result resultCode, Android.Content.Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			if (requestCode == OPEN_DIRECTORY_REQUEST_CODE && resultCode == Result.Ok) {
				Activity.ContentResolver
				        .TakePersistableUriPermission (data.Data, ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);
				UpdateDirectoryEntries (data.Data);
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.fragment_scoped_dir_access, container, false);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);

			currentDirectoryTextView = (TextView)view.FindViewById (Resource.Id.textview_current_directory);
			nothingInDirectoryTextView = (TextView)view.FindViewById (Resource.Id.textview_nothing_in_directory);

			var openPictureButton = (Button)view.FindViewById (Resource.Id.button_open_directory_primary_volume);
			openPictureButton.Click += delegate {
				string selected = directoriesSpinner.SelectedItem.ToString ();
				string directoryName = GetDirectoryName (selected);
				Intent intent = storageManager.PrimaryStorageVolume.CreateAccessIntent (directoryName);
				StartActivityForResult (intent, OPEN_DIRECTORY_REQUEST_CODE);
			};

			// Set onClickListener for the external volumes if exists
			var containerVolumes = (LinearLayout)Activity.FindViewById (Resource.Id.container_volumes);
			foreach (StorageVolume volume in storageManager.StorageVolumes) {
				if (volume.IsPrimary) {
					// Primary volume area is already added
					continue;
				}
				var volumeArea = (LinearLayout)Activity.LayoutInflater.Inflate (
					Resource.Layout.volume_entry, containerVolumes);
				var volumeName = (TextView)volumeArea.FindViewById (Resource.Id.textview_volume_name);
				volumeName.Text = volume.GetDescription (Activity);
				var button = (Button)volumeArea.FindViewById (Resource.Id.button_open_directory);
				button.Click += delegate {
					string selected = directoriesSpinner.SelectedItem.ToString ();
					string directoryName = GetDirectoryName (selected);
					Intent intent = volume.CreateAccessIntent (directoryName);
					StartActivityForResult (intent, OPEN_DIRECTORY_REQUEST_CODE);
				};
			}

			var recyclerView = (RecyclerView)view.FindViewById (Resource.Id.recyclerview_directory_entries);
			if (savedInstanceState != null) {
				directoryEntries = (List<DirectoryEntry>)savedInstanceState.GetParcelableArrayList (DIRECTORY_ENTRIES_KEY);
				currentDirectoryTextView.Text = savedInstanceState.GetString (SELECTED_DIRECTORY_KEY);
				adapter = new DirectoryEntryAdapter (directoryEntries);

				if (adapter.ItemCount == 0)
					nothingInDirectoryTextView.Visibility = ViewStates.Visible;
				
			} else {
				directoryEntries = new List<DirectoryEntry> ();
				adapter = new DirectoryEntryAdapter ();
			}

			recyclerView.SetAdapter (adapter);
			recyclerView.SetLayoutManager (new LinearLayoutManager (Activity));
			directoriesSpinner = (Spinner)view.FindViewById (Resource.Id.spinner_directories);
			var directoriesAdapter = ArrayAdapter.CreateFromResource (Activity, Resource.Array.directories,
																	  Android.Resource.Layout.SimpleSpinnerDropDownItem);
			directoriesSpinner.Adapter = directoriesAdapter;
		}

		public override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);

			outState.PutString (SELECTED_DIRECTORY_KEY, currentDirectoryTextView.Text);
			var entries = new List<IParcelable> (directoryEntries.Count);
			foreach (DirectoryEntry de in directoryEntries)
				entries.Add (de);
			outState.PutParcelableArrayList (DIRECTORY_ENTRIES_KEY, entries);
	 	}

		void UpdateDirectoryEntries (Uri uri)
		{
			directoryEntries.Clear ();
			var contentResolver = Activity.ContentResolver;
			Uri docUri = DocumentsContract.BuildDocumentUriUsingTree (uri, DocumentsContract.GetTreeDocumentId (uri));
			Uri childrenUri = DocumentsContract.BuildChildDocumentsUriUsingTree (uri, DocumentsContract.GetTreeDocumentId (uri));

			try {
				using (ICursor docCursor = contentResolver.Query (docUri, DIRECTORY_SELECTION, null, null, null)) {
					while (docCursor != null && docCursor.MoveToNext ()) {
						currentDirectoryTextView.Text = docCursor.GetString (
							docCursor.GetColumnIndex (DocumentsContract.Document.ColumnDisplayName));
					}
				}
			} catch {
			}

			try {
				using (ICursor childCursor = contentResolver.Query (childrenUri, DIRECTORY_SELECTION, null, null, null)) {
					while (childCursor != null && childCursor.MoveToNext ()) {
						var entry = new DirectoryEntry ();
						entry.FileName = childCursor.GetString (childCursor.GetColumnIndex (DocumentsContract.Document.ColumnDisplayName));
						entry.MimeType = childCursor.GetString (childCursor.GetColumnIndex (DocumentsContract.Document.ColumnMimeType));
						directoryEntries.Add (entry);
					}
				}

				if (directoryEntries.Count == 0)
					nothingInDirectoryTextView.Visibility = ViewStates.Visible;
				else
					nothingInDirectoryTextView.Visibility = ViewStates.Gone;

				adapter.DirectoryEntries = directoryEntries;
				adapter.NotifyDataSetChanged ();
			} catch {
			}
		}

		string GetDirectoryName (string name)
		{
			switch (name) {
			case "ALARMS":
				return Environment.DirectoryAlarms;
			case "DCIM":
				return Environment.DirectoryDcim;
			case "DOCUMENTS":
				return Environment.DirectoryDocuments;
			case "DOWNLOADS":
				return Environment.DirectoryDownloads;
			case "MOVIES":
				return Environment.DirectoryMovies;
			case "MUSIC":
				return Environment.DirectoryMusic;
			case "NOTIFICATIONS":
				return Environment.DirectoryNotifications;
			case "PICTURES":
				return Environment.DirectoryPictures;
			case "PODCASTS":
				return Environment.DirectoryPodcasts;
			case "RINGTONES":
				return Environment.DirectoryRingtones;
			default:
				throw new System.ArgumentException ("Invalid directory representation: " + name);
			}
		}
	}
}

