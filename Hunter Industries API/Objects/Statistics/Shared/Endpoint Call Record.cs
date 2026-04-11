// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Statistics.Shared
{
    /// <summary>
    /// </summary>
    public class EndpointCallRecord
    {
        /// <summary>
        /// The name of the endpoint.
        /// </summary>
        public string Endpoint { get; set; }
        /// <summary>
        /// The number of calls made.
        /// </summary>
        public int Calls { get; set; }
    }
}