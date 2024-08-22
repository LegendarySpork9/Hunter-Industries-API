// Copyright © - unpublished - Toby Hunter
namespace HunterIndustriesAPI.Models.Responses
{
    public class ResponseModel
    {
        public int StatusCode { get; set; } = 500;
        public object Data { get; set; } = new { error = "An unknown error occured. Please raise this with the time the error occured so it can be investigated." };
    }
}
