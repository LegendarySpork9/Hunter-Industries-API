// Copyright © - unpublished - Toby Hunter
using System.Text.Json.Serialization;

namespace HunterIndustriesAPI.Models.Responses
{
    public class ResponseModel
    {
        [JsonIgnore]
        public int StatusCode { get; set; } = 500;
        public object Data { get; set; } = new { error = "An unknown error occured. Please raise this with the time the error occured so it can be investigated." };
    }
}
