// Copyright © 11/06/2026 Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Services;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HunterIndustriesAPIControlPanel.Components.Pages.AuditHistory
{
    public partial class AuditHistoryDetail
    {
        [Inject]
        private IConfigurableLoggerService _Logger { get; set; } = default!;
        [Inject]
        private APIService APIService { get; set; } = default!;

        [Parameter]
        public int Id { get; set; }

        [SupplyParameterFromQuery(Name = "fromPage")]
        public string? QueryFromPage { get; set; }
        [SupplyParameterFromQuery(Name = "fromPageParams")]
        public string? QueryFromPageParameters { get; set; }

        private AuditHistoryModel? AuditHistory;

        private bool IsLoading;

        private string ReturnURL = "/audit-history";

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                "Opened Audit History Detail Page");

            IsLoading = true;

            if (!string.IsNullOrWhiteSpace(QueryFromPage))
            {
                ReturnURL = $"/{QueryFromPage}";

                if (QueryFromPage == "dashboard")
                {
                    ReturnURL = "/";
                }

                if (!string.IsNullOrWhiteSpace(QueryFromPageParameters))
                {
                    ReturnURL += $"?{QueryFromPageParameters.Replace("[", "")
                        .Replace("]", "")}";
                }
            }

            AuditHistory = await APIService.GetAuditHistory(Id);

            IsLoading = false;
        }

        /// <summary>
        /// Returns a syntax-highlighted HTML representation of the JSON string.
        /// </summary>
        private static MarkupString FormatJson(string json)
        {
            const int maxLength = 100000;

            try
            {
                JToken token = JToken.Parse(json);
                json = token.ToString(Formatting.Indented);
            }

            catch (JsonReaderException)
            {

            }

            if (json.Length > maxLength)
            {
                string truncated = json[..maxLength];
                return new MarkupString($"{System.Net.WebUtility.HtmlEncode(truncated)}\n\n<span style=\"color:#808080\">... truncated ({json.Length:N0} characters total)</span>");
            }

            StringBuilder sb = new();

            string pattern = @"(""[^""\\]*(?:\\.[^""\\]*)*"")\s*:|""[^""\\]*(?:\\.[^""\\]*)*""|-?\d+(?:\.\d+)?(?:[eE][+-]?\d+)?|true|false|null|[{}\[\]:,]";

            foreach (Match match in Regex.Matches(json, pattern))
            {
                string token = match.Value;

                if (token.EndsWith(':') && token.StartsWith('\"'))
                {
                    string key = token[..^1];
                    sb.Append($"<span style=\"color:#9CDCFE\">{System.Net.WebUtility.HtmlEncode(key)}</span>");
                    sb.Append("<span style=\"color:#D4D4D4\">:</span>");
                }

                else if (token.StartsWith('\"'))
                {
                    sb.Append($"<span style=\"color:#CE9178\">{System.Net.WebUtility.HtmlEncode(token)}</span>");
                }

                else if (token == "true" || token == "false")
                {
                    sb.Append($"<span style=\"color:#569CD6\">{token}</span>");
                }

                else if (token == "null")
                {
                    sb.Append($"<span style=\"color:#808080\">{token}</span>");
                }

                else if (char.IsDigit(token[0]) || token[0] == '-')
                {
                    sb.Append($"<span style=\"color:#B5CEA8\">{token}</span>");
                }

                else
                {
                    sb.Append($"<span style=\"color:#D4D4D4\">{token}</span>");
                }

                int endPos = match.Index + match.Length;
                int nextMatchStart = match.NextMatch().Success ? match.NextMatch().Index : json.Length;
                string whitespace = json[endPos..nextMatchStart];

                if (!string.IsNullOrEmpty(whitespace))
                {
                    sb.Append(System.Net.WebUtility.HtmlEncode(whitespace));
                }
            }

            return new MarkupString(sb.ToString());
        }
    }
}
