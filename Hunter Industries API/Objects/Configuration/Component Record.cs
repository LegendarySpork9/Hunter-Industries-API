// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Configuration
{
    /// <summary>
    /// </summary>
    public class ComponentRecord
    {
        /// <summary>
        /// Id of the record.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of the component.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Whether the record is deleted.
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}