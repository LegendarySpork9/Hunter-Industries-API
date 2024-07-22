// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Objects.Assistant;

namespace HunterIndustriesAPI.Models.Responses.Assistant
{
    public record ConfigResponseModel
    {
        public string? LatestRelease { get; set; }
        public List<AssistantConfiguration>? AssistantConfigurations { get; set; }
        public int? ConfigCount { get; set; }
        public int? TotalCount { get; set; }
    }
}
