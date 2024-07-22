// Copyright © - unpublished - Toby Hunter
namespace HunterIndustriesAPI.Models.Responses.Assistant
{
    public record VersionResponseModel
    {
        public string? AssistantName { get; set; }
        public string? IdNumber { get; set; }
        public string? Version { get; set; }
    }
}
