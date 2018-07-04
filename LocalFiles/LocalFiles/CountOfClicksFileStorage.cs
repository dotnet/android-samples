using System.IO;
using System.Threading.Tasks;

using Android.Widget;

namespace LocalFiles
{
    /// <summary>
    ///     This class is responsible for saving and loading the number of clicks to a file
    ///     on the device.
    /// </summary>
    public class CountOfClicksFileStorage : ICountFileStorage
    {
        protected static readonly string DefaultFilename = "count.txt";

        /// <summary>
        ///     Holds the number of times that the button as been clicked.
        /// </summary>
        public int Count { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     Load the contents of the file from the Android file system. Uses
        ///     the async .NET APIs for reading bytes from a file.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> LoadFileAsync(IGenerateNameOfFile fileName)
        {
            var backingFile = fileName.GetAbsolutePathToFile(DefaultFilename);
            if (backingFile == null || !File.Exists(backingFile))
            {
                return 0;
            }

            var count = 0;
            using (var reader = new StreamReader(backingFile, true))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (int.TryParse(line, out var newcount))
                    {
                        count = newcount;
                    }
                }
            }

            return count;
        }

        /// <summary>
        ///     Writes the number to a text file  using the async
        ///     .NET APIs for write the contents to the filesystem.
        /// </summary>
        /// <param name="fileName">The name (but not the path) of the text file that will be written to.</param>
        /// <param name="count">A number that will be written to a text file.</param>
        /// <returns></returns>
        public virtual async Task WriteFileAsync(IGenerateNameOfFile fileName, int count)
        {
            var backingFile = fileName.GetAbsolutePathToFile(DefaultFilename);
            if (backingFile == null)
            {
                // For reasons beyond the control/responsibilty of this class,
                // there is no path to the backing storage file. Don't do anything.
                return;
            }

            using (var writer = File.CreateText(backingFile))
            {
                await writer.WriteLineAsync(count.ToString());
            }
        }

        /// <summary>
        ///     Will display th name of the backing text file in the specified TextView.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="view"></param>
        public void DisplayPathIn(IGenerateNameOfFile fileName, TextView view)
        {
            view.Text = fileName.GetAbsolutePathToFile(DefaultFilename);
        }

        /// <summary>
        ///     Resets the count to zero and will delete the backing text file.
        /// </summary>
        /// <param name="filename"></param>
        public Task DeleteAsync(IGenerateNameOfFile filename)
        {
            Count = 0;
            return Task.Run(() => { File.Delete(filename.GetAbsolutePathToFile(DefaultFilename)); });
        }
    }
}
