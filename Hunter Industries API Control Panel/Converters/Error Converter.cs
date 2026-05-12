// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Converters
{
    public static class ErrorConverter
    {
        /// <summary>
        /// Returns the class name from the given string.
        /// </summary>
        public static string ExtractClassMethod(string summary)
        {
            string className = string.Empty;

            string[] words = summary.TrimEnd('.').Split(' ');

            for (int i = words.Length - 1; i >= 0; i--)
            {
                if (words[i].Contains('.'))
                {
                    className = words[i];
                    break;
                }
            }

            if (string.IsNullOrEmpty(className))
            {
                if (summary.Length > 40)
                {
                    className = $"{summary[..40]}...";
                }

                else
                {
                    className = summary;
                }
            }

            return className;
        }
    }
}
