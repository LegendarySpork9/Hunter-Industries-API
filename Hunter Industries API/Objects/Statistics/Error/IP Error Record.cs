// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Statistics.Error
{
    /// <summary>
    /// </summary>
    public class IPErrorRecord
    {
        /// <summary>
        /// The ip address of the failed call.
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// The number of errors.
        /// </summary>
        public int Errors { get; set; }
    }
}
