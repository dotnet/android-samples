using System;
using System.IO;

using Android.Content;

namespace LocalFiles
{
    /// <summary>
    ///     Used to determine the path to the backing text file on external storage.
    ///     By default, the file will be considered <i>private</i>.
    /// </summary>
    public class ExternalStorageFilenameGenerator : IGenerateNameOfFile
    {
        readonly WeakReference<Context> contextRef;
        readonly string directoryType;

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="publicStorage"></param>
        /// <param name="directoryType">One of the Android directories for media storage. Can be null for private files, and will default to <c>Android.OS.Environment.DirectoryDocuments</c> for public files.</param>
        public ExternalStorageFilenameGenerator(Context context, bool publicStorage = false, string directoryType = null)
        {
            contextRef = new WeakReference<Context>(context);
            PublicStorage = publicStorage;
            this.directoryType = string.IsNullOrWhiteSpace(directoryType) ? null : directoryType.Trim();
        }

        public bool PublicStorage { get; }

        public string GetAbsolutePathToFile(string fileName)
        {
            string dir;
            if (PublicStorage)
            {
                if (string.IsNullOrWhiteSpace(directoryType))
                {
                    dir = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                }
                else
                {
                    dir = Android.OS.Environment.GetExternalStoragePublicDirectory(directoryType).AbsolutePath;
                }
            }
            else
            {
                if (!contextRef.TryGetTarget(out var c))
                {
                    return null;
                }

                dir = c.GetExternalFilesDir(directoryType).AbsolutePath;
            }

            return Path.Combine(dir, fileName);
        }
    }
}