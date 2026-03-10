namespace HunterIndustriesAPI.Abstractions
{
    /// <summary>
    /// Interface for the file system operations.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// </summary>
        string ReadAllText(string path);
    }
}
