// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the method call numbers.
    /// </summary>
    public class MethodCallModel
    {
        public required string Method { get; set; }
        public required int Calls { get; set; }
    }
}
