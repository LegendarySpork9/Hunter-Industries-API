using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace HunterIndustriesAPI.Tests.Functions
{
    internal static class JSONLoaderFunction
    {
        // Loads the given JSON file and returns the JSON.
        public static JObject LoadJSON(string file)
        {
            string directory = Directory.GetCurrentDirectory().Replace(@"bin\Debug", "");
            string path = Path.Combine(directory, @"Mocks\Models", file);

            StreamReader stream = File.OpenText(path);
            JsonTextReader reader = new JsonTextReader(stream);
            JObject json = JObject.Load(reader);

            return json;
        }
    }
}
