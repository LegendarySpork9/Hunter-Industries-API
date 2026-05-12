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
                "Byte" => "Byte (0 -> 255)",
                "Decimal" => "Currancy",
                "Double" => "Math Integer",
                "Int16" => "Integer (~32 KB)",
                "Int32" => "Integer (~2 GB)",
                "Int64" => "Integer (~8 EB)",
                "Int128" => "Integer (~139 BB)",
                "SByte" => "SByte (-128 -> 127)",
                "Single" => "Float",
                "TimeSpan" => "Duration",
                _ => type
            };
        }
    }
}
