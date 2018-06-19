using System.Collections.Generic;
using System.IO;
using System.Linq;

using Android.Content;
using Android.Views;
using Android.Widget;

namespace com.xamarin.recipes.filepicker
{
    public class FileListAdapter : ArrayAdapter<FileSystemInfo>
    {
        readonly Context _context;

        public FileListAdapter(Context context, IList<FileSystemInfo> fsi)
            : base(context, Resource.Layout.file_picker_list_item, Android.Resource.Id.Text1, fsi)
        {
            _context = context;
        }

        /// <summary>
        ///     We provide this method to get around some of the
        /// </summary>
        /// <param name="directoryContents"> </param>
        public void AddDirectoryContents(IEnumerable<FileSystemInfo> directoryContents)
        {
            Clear();
            // Notify the _adapter that things have changed or that there is nothing 
            // to display.
            if (directoryContents.Any())
            {
                lock (this)
                {
                    foreach (var fsi in directoryContents)
                    {
                        Add(fsi);
                    }
                }

                NotifyDataSetChanged();
            }
            else
            {
                NotifyDataSetInvalidated();
            }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var fileSystemEntry = GetItem(position);

            FileListRowViewHolder viewHolder;
            View row;
            if (convertView == null)
            {
                row = _context.GetLayoutInflater().Inflate(Resource.Layout.file_picker_list_item, parent, false);
                viewHolder = new FileListRowViewHolder(row.FindViewById<TextView>(Resource.Id.file_picker_text),
                                                       row.FindViewById<ImageView>(Resource.Id.file_picker_image));
                row.Tag = viewHolder;
            }
            else
            {
                row = convertView;
                viewHolder = (FileListRowViewHolder) row.Tag;
            }

            viewHolder.Update(fileSystemEntry.Name, fileSystemEntry.IsDirectory() ? Resource.Drawable.folder : Resource.Drawable.file);

            return row;
        }
    }
}
