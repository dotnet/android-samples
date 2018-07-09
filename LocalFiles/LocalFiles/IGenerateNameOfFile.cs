namespace LocalFiles
{
    /// <summary>
    ///     Implemented by classes that will determine the absolute path for
    ///     a file.
    /// </summary>
    public interface IGenerateNameOfFile
    {
        /// <summary>
        ///     Generates an absolute path for the given filename.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>
        ///     The full path to the file as a string.
        ///     <code>null</code> if it is not possible to determine the path.
        /// </returns>
        string GetAbsolutePathToFile(string fileName);
    }
}
