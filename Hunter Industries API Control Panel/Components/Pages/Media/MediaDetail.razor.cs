// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Services;
using Microsoft.AspNetCore.Components;

namespace HunterIndustriesAPIControlPanel.Components.Pages.Media
{
    public partial class MediaDetail
    {
        [Inject]
        private IConfigurableLoggerService _Logger { get; set; } = default!;
        [Inject]
        private APIService APIService { get; set; } = default!;

        [Parameter]
        public int Id { get; set; }

        private MediaModel? Media;

        private bool IsLoading;

        private string ReturnURL = "/media";

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                "Opened Media Detail Page");

            IsLoading = true;

            Media = await APIService.GetMedia(Id);
            
            if (Media != null)
            {
                ReturnURL = $"/media?application={Media.Application}";
            }

            IsLoading = false;
        }
    }
}
