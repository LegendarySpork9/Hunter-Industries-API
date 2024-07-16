// Copyright © - unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects
{
    public record TokenInfo
    {
        public string? ApplicationName { get; set; }
        public IEnumerable<string>? Scope { get; set; }
        public DateTime? Issued { get; set; }
        public DateTime? Expires { get; set; }
    }
}
