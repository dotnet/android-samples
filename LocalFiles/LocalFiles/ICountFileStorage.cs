using System.Threading.Tasks;

namespace LocalFiles
{
    /// <summary>
    /// Interface implemented by class that will perist an intenger to a
    /// file somewhere on the filesystem.
    /// </summary>
    public interface ICountFileStorage
    {
        /// <summary>
        ///     Read the integer from a text file. 
        /// </summary>
        /// <returns></returns>
        Task<int> ReadFileAsync(IGenerateNameOfFile fileName);

        /// <summary>
        ///     Writes the number to a text file.
        /// </summary>
        /// <param name="fileName">A class that will determine the path of the file that should be written to.</param>
        /// <param name="count">A number that will be written to a text file.</param>
        /// <returns></returns>
        Task WriteFileAsync(IGenerateNameOfFile fileName, int count);

        /// <summary>
        ///     Resets the count to zero and will delete the backing text file.
        /// </summary>
        /// <param name="filename"></param>
        Task DeleteFileAsync(IGenerateNameOfFile filename);

        /// <summary>
        ///     Holds the number of times that the button as been clicked.
        /// </summary>
        int Count { get; set; }
    }
}