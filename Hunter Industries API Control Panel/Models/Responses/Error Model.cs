// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the error log api response.
    /// </summary>
    public class ErrorModel
    {
        public required int Id { get; set; }
        public required DateTime DateOccured { get; set; }
        public required string IPAddress { get; set; }
        public required string Summary { get; set; }
        public required string Message { get; set; }
    }
}
