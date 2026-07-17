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
        /// The type of filter (tag, numeric, text, boolean, null).
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// The comparison operator for the filter.
        /// </summary>
        public string Operator { get; set; }
        /// <summary>
        /// The dot-notation path to the item property.
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// The values the filter allows.
        /// </summary>
        public string Values { get; set; }
    }
}