using Newtonsoft.Json;

namespace HunterIndustriesAPI.Models.Responses
{
    /// <summary>
    /// </summary>
    public class ResponseModel
    {
        /// <summary>
        /// The resulting status code.
        /// </summary>
        [JsonIgnore]
        public int StatusCode { get; set; } = 500;
        /// <summary>
        /// The resulting message or model.
        /// </summary>
        public object Data { get; set; } = new { error = "An unknown error occured. Please raise this with the time the error occured so it can be investigated." };
    }
}