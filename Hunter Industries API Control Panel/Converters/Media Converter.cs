// Copyright © - 29/06/2026 - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Converters
{
    public static class MediaConverter
    {
        /// <summary>
        /// Adds the size characters to the file size for ease of reading.
        /// </summary>
        public static string FormatFileSize(long bytes)
        {
            string[] sizes =
            [
                "B",
                "KB",
                "MB",
                "GB"
            ];
            int order = 0;
            double size = bytes;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }
    }
}
