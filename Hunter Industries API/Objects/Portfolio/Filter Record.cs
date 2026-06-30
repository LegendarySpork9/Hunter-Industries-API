// Copyright © - Unpublished - Toby Hunter
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
        /// The values the filter allows.
        /// </summary>
        public string Values { get; set; }
        /// <summary>
        /// Whether the record is deleted.
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}