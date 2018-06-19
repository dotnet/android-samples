using Android.Widget;

using Java.Lang;

namespace com.xamarin.recipes.filepicker
{
    /// <summary>
    ///     This class is used to hold references to the views contained in a list row.
    /// </summary>
    /// <remarks>
    ///     This is an optimization so that we don't have to always look up the
    ///     ImageView and the TextView for a given row in the ListView.
    /// </remarks>
    public class FileListRowViewHolder : Object
    {
        public FileListRowViewHolder(TextView textView, ImageView imageView)
        {
            TextView = textView;
            ImageView = imageView;
        }

        public ImageView ImageView { get; }
        public TextView TextView { get; }

        /// <summary>
        ///     This method will update the TextView and the ImageView that are
        ///     are
        /// </summary>
        /// <param name="fileName"> </param>
        /// <param name="fileImageResourceId"> </param>
        public void Update(string fileName, int fileImageResourceId)
        {
            TextView.Text = fileName;
            ImageView.SetImageResource(fileImageResourceId);
        }
    }
}
