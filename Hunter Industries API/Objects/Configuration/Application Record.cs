// Copyright © - Unpublished - Toby Hunter
using System.Collections.Generic;

namespace HunterIndustriesAPI.Objects.Configuration
{
    /// <summary>
    /// </summary>
    public class ApplicationRecord
    {
        /// <summary>
        /// Id of the record.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of the application.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The authorisation record.
        /// </summary>
        public AuthorisationRecord Authorisation { get; set; }
        /// <summary>
        /// The setting records.
        /// </summary>
        public List<ApplicationSettingRecord> Settings { get; set; }
        /// <summary>
        /// Whether the record is deleted.
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}