// Copyright © - Unpublished - Toby Hunter
using System.Collections.Generic;

namespace HunterIndustriesAPI.Objects.Portfolio
{
    /// <summary>
    /// </summary>
    public class FilterRecord
    {
        /// <summary>
        /// Id of the record.
        /// </summary>
        public int Id { get; set; }
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
        public List<string> Values { get; set; }
        /// <summary>
        /// Whether the record is deleted.
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}