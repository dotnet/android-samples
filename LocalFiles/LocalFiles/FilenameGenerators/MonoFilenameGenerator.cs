using System;
using System.IO;

namespace LocalFiles
{
    /// <summary>
    ///     This class is used to create the path to a file on internal storage
    ///     using the .NET APIs.
    /// </summary>
    public class MonoFilenameGenerator : IGenerateNameOfFile
    {
        public string GetAbsolutePathToFile(string fileName)
        {
            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            return Path.Combine(dir, fileName);
        }
    }
}