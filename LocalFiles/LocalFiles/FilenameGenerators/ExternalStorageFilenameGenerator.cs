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
            this.directoryType = SanitizeDirectoryType(directoryType);
        }

        public bool PublicStorage { get; }

        public string GetAbsolutePathToFile(string fileName)
        {
            string dir;
            if (PublicStorage)
            {
                dir = Android.OS.Environment.GetExternalStoragePublicDirectory(directoryType).AbsolutePath;
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

        /// <summary>
        /// Makes sure that the directory type is valid for the type of storage.
        /// </summary>
        /// <param name="requestedDirectoryType"></param>
        /// <returns></returns>
        string SanitizeDirectoryType(string requestedDirectoryType)
        {
            string dirType;
            if (string.IsNullOrEmpty(requestedDirectoryType))
            {
                dirType = PublicStorage ? Android.OS.Environment.DirectoryDocuments : null;
            }
            else
            {
                dirType = requestedDirectoryType.Trim();
            }

            return dirType;
        }
    }
}