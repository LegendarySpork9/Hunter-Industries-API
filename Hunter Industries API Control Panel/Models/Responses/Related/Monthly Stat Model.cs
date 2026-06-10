// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the monthly call numbers.
    /// </summary>
    public class MonthlyStatModel
    {
        public required int ThisMonth { get; set; }
        public required int LastMonth { get; set; }
    }
}
