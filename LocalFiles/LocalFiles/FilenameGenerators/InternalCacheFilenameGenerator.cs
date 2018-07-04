using System;
using System.IO;

using Android.Content;

namespace LocalFiles
{
    /// <summary>
    /// This class will generate a filename for a temp/cache file
    /// </summary>
    public class InternalCacheFilenameGenerator : IGenerateNameOfFile
    {
        readonly WeakReference<Context> contextRef;

        public InternalCacheFilenameGenerator(Context context)
        {
            contextRef = new WeakReference<Context>(context);
        }

        public string GetAbsolutePathToFile(string fileName)
        {
            if (!contextRef.TryGetTarget(out var c))
            {
                return null;
            }

            
            var dir = c.CacheDir.AbsolutePath;

            return Path.Combine(dir, fileName);
        }
    }
}