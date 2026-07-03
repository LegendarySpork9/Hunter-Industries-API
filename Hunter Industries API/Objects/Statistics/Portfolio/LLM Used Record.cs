// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Statistics.Portfolio
{
    /// <summary>
    /// </summary>
    public class LLMUsedRecord
    {
        /// <summary>
        /// The company that owns the model.
        /// </summary>
        public string Company { get; set; }
        /// <summary>
        /// The name of the model.
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// The number of times the LLM has been used.
        /// </summary>
        public int Uses { get; set; }
    }
}