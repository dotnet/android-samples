using System;
using System.IO;

using Android.Content;

namespace LocalFiles
{
    /// <summary>
    ///     This class is used to create the path to a file on internal storage
    ///     using the Android APIs.
    /// </summary>
    public class InternalStorageFilenameGenerator : IGenerateNameOfFile
    {
        readonly WeakReference<Context> contextRef;

        public InternalStorageFilenameGenerator(Context context)
        {
            contextRef = new WeakReference<Context>(context);
        }

        public string GetAbsolutePathToFile(string fileName)
        {
            if (!contextRef.TryGetTarget(out var c))
            {
                return null;
            }

            var dir = c.FilesDir.AbsolutePath;
            return Path.Combine(dir, fileName);
        }
    }
}