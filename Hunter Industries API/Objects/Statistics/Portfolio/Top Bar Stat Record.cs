// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Statistics.Portfolio
{
    /// <summary>
    /// </summary>
    public class TopBarStatRecord
    {
        /// <summary>
        /// The number of items registered.
        /// </summary>
        public int Items { get; set; }
        /// <summary>
        /// The number of filters registered.
        /// </summary>
        public int Filters { get; set; }
        /// <summary>
        /// The number of items where LLMs have been used.
        /// </summary>
        public int AIUsed { get; set; }
    }
}
