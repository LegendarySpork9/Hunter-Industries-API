// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Functions
{
    /// <summary>
    /// </summary>
    public static class ResponseFunction
    {
        /// <summary>
        /// Converts the object to a JSON string.
        /// </summary>
        public static string GetModelJSON(object model)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(model);
        }
    }
}