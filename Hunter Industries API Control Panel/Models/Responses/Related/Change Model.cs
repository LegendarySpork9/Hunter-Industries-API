// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the change data.
    /// </summary>
    public class ChangeModel
    {
        public required int Id { get; set; }
        public required string Field { get; set; }
        public required string OldValue { get; set; }
        public required string NewValue { get; set; }
    }
}
