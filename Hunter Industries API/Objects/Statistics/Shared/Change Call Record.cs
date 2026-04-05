// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Statistics.Shared
{
    /// <summary>
    /// </summary>
    public class ChangeCallRecord
    {
        /// <summary>
        /// The name of the field.
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// The number of changes made.
        /// </summary>
        public int Calls { get; set; }
    }
}