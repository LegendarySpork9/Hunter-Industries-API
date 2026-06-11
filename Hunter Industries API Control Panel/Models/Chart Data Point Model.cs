// Copyright © - 11/06/2026 - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models
{
    /// <summary>
    /// Stores the label and value for a chart data point.
    /// </summary>
    public class ChartDataPointModel
    {
        public required string Label { get; set; }
        public required int Value { get; set; }
    }
}
