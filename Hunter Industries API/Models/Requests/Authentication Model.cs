// Copyright © - unpublished - Toby Hunter
using Microsoft.Extensions.Primitives;
using System.ComponentModel.DataAnnotations;

namespace HunterIndustriesAPI.Models.Requests
{
    public class AuthenticationModel
    {
        [Required]
        public string? Phrase { get; set; }
        [Required]
        public StringValues? AuthHeader { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
