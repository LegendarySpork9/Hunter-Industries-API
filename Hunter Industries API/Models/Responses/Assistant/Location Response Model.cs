// Copyright © - unpublished - Toby Hunter
namespace HunterIndustriesAPI.Models.Responses.Assistant
{
    public record LocationResponseModel
    {
        public string? AssistantName { get; set; }
        public string? IdNumber { get; set; }
        public string? HostName { get; set; }
        public string? IPAddress { get; set; }
    }
}
