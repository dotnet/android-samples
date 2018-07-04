using System.Threading.Tasks;

namespace LocalFiles
{
    public interface ICountFileStorage
    {
        /// <summary>
        ///     Load the contents of the file from the Android file system. Uses
        ///     the async .NET APIs for reading bytes from a file.
        /// </summary>
        /// <returns></returns>
        Task<int> LoadFileAsync(IGenerateNameOfFile fileName);

        /// <summary>
        ///     Writes the number to a text file  using the async
        ///     .NET APIs for write the contents to the filesystem.
        /// </summary>
        /// <param name="fileName">The name (but not the path) of the text file that will be written to.</param>
        /// <param name="count">A number that will be written to a text file.</param>
        /// <returns></returns>
        Task WriteFileAsync(IGenerateNameOfFile fileName, int count);

        /// <summary>
        ///     Resets the count to zero and will delete the backing text file.
        /// </summary>
        /// <param name="filename"></param>
        Task DeleteAsync(IGenerateNameOfFile filename);
    }
}