// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPICommon.Abstractions
{
    /// <summary>
    /// Interface for the file system operations.
    /// </summary>
    public interface IFileSystem
    {
        string ReadAllText(string path);
    }
}
