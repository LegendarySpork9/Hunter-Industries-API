// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Models.Requests.Filters
{
    /// <summary>
    /// </summary>
    public class UserFilterModel
    {
        /// <summary>
        /// The id number of the user.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of the user.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Whether to return deleted users.
        /// </summary>
        public bool IncludeDeleted { get; set; }
    }
}