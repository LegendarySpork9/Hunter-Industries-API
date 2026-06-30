// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Models.Requests.Bodies.Portfolio
{
    /// <summary>
    /// </summary>
    public class FilterModel
    {
        /// <summary>
        /// The name of the filter.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The values the filter allows.
        /// </summary>
        public string Values { get; set; }
    }
}