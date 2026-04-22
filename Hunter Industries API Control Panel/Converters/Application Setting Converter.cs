// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Converters
{
    public static class ApplicationSettingConverter
    {
        /// <summary>
        /// Returns the UI type for the given database type.
        /// </summary>
        public static string GetDataType(string type)
        {
            return type switch
            {
                "Int16" => "Integer (16 x 2,048)",
                "Int32" => "Integer (32 x 4,096)",
                "Int64" => "Integer (64 x 8,192)",
                "Single" => "Float",
                "TimeSpan" => "Duration",
                _ => type
            };
        }
    }
}
