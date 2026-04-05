// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Statistics.Shared
{
    /// <summary>
    /// </summary>
    public class MethodCallRecord
    {
        /// <summary>
        /// The name of the method.
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// The number of calls made.
        /// </summary>
        public int Calls { get; set; }
    }
}