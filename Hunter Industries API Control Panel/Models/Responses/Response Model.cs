// Copyright © - 11/06/2026 - Toby Hunter
using System.Net;

namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the api response.
    /// </summary>
    public class ResponseModel
    {
        public required HttpStatusCode StatusCode { get; set; }
        public required string Message { get; set; }
    }
}
