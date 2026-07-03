// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Models.Requests.Bodies.Portfolio
{
    /// <summary>
    /// </summary>
    public class ItemMetricModel
    {
        /// <summary>
        /// Id of the record.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The metric being logged.
        /// </summary>
        public string Metric { get; set; }
    }
}