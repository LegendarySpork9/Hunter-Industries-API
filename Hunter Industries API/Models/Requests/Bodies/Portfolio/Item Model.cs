// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Objects.Portfolio;
using System.Collections.Generic;

namespace HunterIndustriesAPI.Models.Requests.Bodies.Portfolio
{
    /// <summary>
    /// </summary>
    public class ItemModel
    {
        /// <summary>
        /// The name of the item.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The type of project.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// The url to the icon used for the item.
        /// </summary>
        public string IconURL { get; set; }
        /// <summary>
        /// The summary of the item.
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// The description of the item.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The frameworks used by the item.
        /// </summary>
        public List<string> Frameworks { get; set; }
        /// <summary>
        /// The languages used by the item.
        /// </summary>
        public List<string> Languages { get; set; }
        /// <summary>
        /// The environments used by the item.
        /// </summary>
        public List<string> Environments { get; set; }
        /// <summary>
        /// The link to the demo environment.
        /// </summary>
        public string DemoURL { get; set; }
        /// <summary>
        /// The latest release notes.
        /// </summary>
        public string ReleaseNotes { get; set; }
        /// <summary>
        /// The build history of the item.
        /// </summary>
        public List<BuildHistoryRecord> BuildHistory { get; set; }
        /// <summary>
        /// The percentage of code covered by unit tests.
        /// </summary>
        public decimal UnitTestCoverage { get; set; }
        /// <summary>
        /// The GitHub information for the project.
        /// </summary>
        public GitHubRecord GitHubInformation { get; set; }
        /// <summary>
        /// How the model has been used.
        /// </summary>
        public string LLMUsageNotes { get; set; }
        /// <summary>
        /// The LLM used for the item.
        /// </summary>
        public LLMRecord LLMUsage { get; set; }
    }
}