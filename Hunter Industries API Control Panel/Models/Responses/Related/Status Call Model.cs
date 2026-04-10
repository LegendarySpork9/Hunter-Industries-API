// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the change call numbers.
    /// </summary>
    public class ChangeCallModel
    {
        public required string Field { get; set; }
        public required int Calls { get; set; }
    }
}
