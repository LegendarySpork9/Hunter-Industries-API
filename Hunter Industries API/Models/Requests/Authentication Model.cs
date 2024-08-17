// Copyright © - unpublished - Toby Hunter
using Microsoft.Extensions.Primitives;

namespace HunterIndustriesAPI.Models.Requests
{
    public class AuthenticationModel
    {
        public string? Phrase { get; set; }
        public StringValues? AuthHeader { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
