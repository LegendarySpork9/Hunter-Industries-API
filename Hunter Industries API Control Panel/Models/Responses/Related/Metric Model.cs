// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the api metric numbers.
    /// </summary>
    public class MetricModel
    {
        public required int Applications { get; set; }
        public required int Users { get; set; }
        public required MonthlyStatModel Calls { get; set; }
        public required MonthlyStatModel LoginAttempts { get; set; }
        public required MonthlyStatModel Changes { get; set; }
        public required MonthlyStatModel Errors { get; set; }
    }
}
