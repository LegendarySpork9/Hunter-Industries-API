// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPICommon.Abstractions
{
    /// <summary>
    /// Interface for the logger service.
    /// </summary>
    public interface IConfigurableLoggerService : ILoggerService
    {
        /// <summary>
        /// </summary>
        void ChangeIdentifier(string value);
    }
}
