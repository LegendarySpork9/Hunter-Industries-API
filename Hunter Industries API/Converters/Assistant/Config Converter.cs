// Copyright © - unpublished - Toby Hunter
namespace HunterIndustriesAPI.Converters.Assistant
{
    public class ConfigConverter
    {
        // Formats the returned data.
        public List<object> FormatData(string[] AssistantNames, string[] AssistantIDs, string[] UserNames, string[] HostNames, bool[] Deletions, string[] Versions)
        {
            var AssistantConfigs = new List<object>();

            for (int x = 0; x < AssistantNames.Length; x++)
            {
                var AssistantConfigRecord = new
                {
                    assistantname = AssistantNames[x],
                    assistantid = AssistantIDs[x],
                    designateduser = UserNames[x],
                    hostname = HostNames[x],
                    deletion = Deletions[x],
                    version = Versions[x]
                };

                AssistantConfigs.Add(AssistantConfigRecord);
            }

            return AssistantConfigs;
        }
    }
}
