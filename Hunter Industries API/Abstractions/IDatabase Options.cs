// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Abstractions
{
    /// <summary>
    /// Interface for the database options.
    /// </summary>
    public interface IDatabaseOptions
    {
        /// <summary>
        /// </summary>
        string ConnectionString { get; }
        /// <summary>
        /// </summary>
        string SQLFiles { get; }
    }
}
