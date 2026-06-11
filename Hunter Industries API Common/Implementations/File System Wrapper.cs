// Copyright © - 11/06/2026 - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;

namespace HunterIndustriesAPICommon.Implementations
{
    public class FileSystemWrapper : IFileSystem
    {
        /// <summary>
        /// Returns the text in a given file.
        /// </summary>
        public string ReadAllText(string path) => File.ReadAllText(path);
    }
}