using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;


namespace AutoBackup
{
	public class MainActivityFragment : Fragment
	{
		public static readonly int ADD_FILE_REQUEST = 1;

		ArrayAdapter<FileInfo> filesArrayAdapter;
		List<FileInfo> files;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			SetHasOptionsMenu (true);
			return inflater.Inflate (Resource.Layout.fragment_main, container, false);
		}

		public override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			if (requestCode == ADD_FILE_REQUEST && resultCode == Result.Ok)
				UpdateListOfFiles ();
		}

		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			// Inflate the menu; this adds items to the action bar if it is present.
			inflater.Inflate (Resource.Menu.menu_main, menu);
			base.OnCreateOptionsMenu (menu, inflater);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			// Handle action bar item clicks here. The action bar will
			// automatically handle clicks on the Home/Up button, so long
			// as you specify a parent activity in AndroidManifest.xml.
			int id = item.ItemId;

			if (id == Resource.Id.action_settings) {
				return true;
			} else if (id == Resource.Id.action_add_file) {
				var addFileIntent = new Intent (Activity, typeof (AddFileActivity));
				StartActivityForResult (addFileIntent, ADD_FILE_REQUEST);
				return true;
			}

			return base.OnOptionsItemSelected (item);
		}

		public override void OnResume ()
		{
			base.OnResume ();
			if (filesArrayAdapter == null) {
				files = CreateListOfFiles ();
				filesArrayAdapter = new FileArrayAdapter (Activity, Resource.Layout.file_list_item, files);

				UpdateListOfFiles ();
				var filesListView = View.FindViewById<ListView> (Resource.Id.file_list);
				filesListView.Adapter = filesArrayAdapter;
			}
		}

		List<FileInfo> CreateListOfFiles ()
		{
			var listOfFiles = new List<FileInfo> ();
			AddFilesToList (listOfFiles, new DirectoryInfo (Activity.FilesDir.AbsolutePath));

			if (Utils.IsExternalStorageAvailable ())
				AddFilesToList (listOfFiles, new DirectoryInfo (Activity.GetExternalFilesDir (null).AbsolutePath));

			AddFilesToList (listOfFiles, new DirectoryInfo (Activity.NoBackupFilesDir.AbsolutePath));
			return listOfFiles;
		}

		void AddFilesToList (List<FileInfo> listOfFiles, DirectoryInfo dir)
		{
			foreach (FileInfo file in dir.EnumerateFiles ())
				listOfFiles.Add (file);
		}

		void UpdateListOfFiles ()
		{
			var emptyFileListMessage = View.FindViewById<TextView> (Resource.Id.empty_file_list_message);
			files = CreateListOfFiles ();

			if (filesArrayAdapter.Count > 0) 
				filesArrayAdapter.Clear ();
			
			foreach (FileInfo file in files)
				filesArrayAdapter.Add (file);
			
			// Display a message instructing to add files if no files found.
			if (files.Count == 0)
				emptyFileListMessage.Visibility = ViewStates.Visible;
			else
				emptyFileListMessage.Visibility = ViewStates.Gone;
		}
	}

	class FileArrayAdapter : ArrayAdapter<FileInfo>
	{
		public FileArrayAdapter (Context context, int resource, IList<FileInfo> objects) : base (context, resource, objects)
		{
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var inflater = LayoutInflater.From (Context);
			View itemView = inflater.Inflate (Resource.Layout.file_list_item, parent, false);

			var fileNameView = itemView.FindViewById <TextView> (Resource.Id.file_name);
			fileNameView.Text = GetItem (position).FullName;

			var fileSize = itemView.FindViewById <TextView> (Resource.Id.file_size);
			fileSize.Text = GetItem (position).Length.ToString ("N0");

			return itemView;
		}

	}
}

