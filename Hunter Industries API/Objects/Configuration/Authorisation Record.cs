// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Configuration
{
    /// <summary>
    /// </summary>
    public class AuthorisationRecord
    {
        /// <summary>
        /// Id of the record.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The authorisation phrase.
        /// </summary>
        public string Phrase { get; set; }
        /// <summary>
        /// Whether the record is deleted.
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}