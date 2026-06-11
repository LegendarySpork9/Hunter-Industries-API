// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Configuration
{
    /// <summary>
    /// </summary>
    public class DomainRecord
    {
        /// <summary>
        /// Id of the record.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The domain URL.
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// Whether the record is deleted.
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}