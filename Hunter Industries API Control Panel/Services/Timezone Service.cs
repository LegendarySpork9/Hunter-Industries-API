// Copyright © - Unpublished - Toby Hunter
using System.Globalization;

namespace HunterIndustriesAPIControlPanel.Services
{
    public class TimezoneService
    {
        public bool ConversionEnabled { get; set; }
        public double OffsetHours { get; set; }

        /// <summary>
        /// Returns the offset label for the columns.
        /// </summary>
        public string GetOffsetLabel()
        {
            double offset = ConversionEnabled ? OffsetHours : 0;
            string sign = offset >= 0 ? "+" : "-";
            double absolute = Math.Abs(offset);
            int hours = (int)absolute;
            int minutes = (int)((absolute - hours) * 60);

            if (minutes > 0)
            {
                return $"UTC{sign}{hours}:{minutes.ToString("D2", CultureInfo.InvariantCulture)}";
            }

            return $"UTC{sign}{hours}";
        }

        /// <summary>
        /// Converts the datetime to users time.
        /// </summary>
        public DateTime ConvertFromUtc(DateTime utcDateTime)
        {
            if (!ConversionEnabled)
            {
                return utcDateTime;
            }

            return utcDateTime.AddHours(OffsetHours);
        }

        /// <summary>
        /// Returns the label for the date column.
        /// </summary>
        public string GetDateLabel(string prefix = "Date")
        {
            return $"{prefix} ({GetOffsetLabel()})";
        }
    }
}
