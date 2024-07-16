// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Objects;

namespace HunterIndustriesAPI.Models.Responses
{
    public record TokenResponseModel
    {
        public string? Type { get; set; }
        public string? Token { get; set; }
        public int? ExpiresIn { get; set; }
        public TokenInfo? Info { get; set; }
    }
}
