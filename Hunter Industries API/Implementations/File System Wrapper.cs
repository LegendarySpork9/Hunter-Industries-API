using HunterIndustriesAPI.Abstractions;
using System.IO;

namespace HunterIndustriesAPI.Implementations
{
    /// <summary>
    /// </summary>
    public class FileSystemWrapper : IFileSystem
    {
        /// <summary>
        /// Returns the text in a given file.
        /// </summary>
        public string ReadAllText(string path) => File.ReadAllText(path);
    }
}