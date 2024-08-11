// Copyright © - unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Assistant
{
    public record AssistantConfiguration
    {
        public string? AssistantName { get; set; }
        public string? IdNumber { get; set; }
        public string? AssignedUser { get; set; }
        public string? HostName { get; set; }
        public bool? Deletion { get; set; }
        public string? Version { get; set; }
    }
}
