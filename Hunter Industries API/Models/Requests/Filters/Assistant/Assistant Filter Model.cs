// Copyright © - unpublished - Toby Hunter
using System.ComponentModel.DataAnnotations;

namespace HunterIndustriesAPI.Models.Requests.Filters.Assistant
{
    public class AssistantFilterModel
    {
        [Required]
        public string? AssistantName { get; set; }
        [Required]
        public string? AssistantId { get; set; }
    }
}
