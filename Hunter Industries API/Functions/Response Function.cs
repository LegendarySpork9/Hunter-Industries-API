namespace HunterIndustriesAPI.Functions
{
    /// <summary>
    /// </summary>
    public class ResponseFunction
    {
        /// <summary>
        /// Converts the object to a JSON string.
        /// </summary>
        public string GetModelJSON(object model)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(model);
        }
    }
}