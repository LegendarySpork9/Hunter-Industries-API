// Copyright © - unpublished - Toby Hunter
namespace HunterIndustriesAPI.Models.Responses.Assistant
{
    public record DeletionResponseModel
    {
        public string? AssistantName { get; set; }
        public string? IdNumber { get; set; }
        public bool? Deletion { get; set; }
    }
}
