// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Models.Requests;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Services;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace HunterIndustriesAPIControlPanel.Components.Pages.Server
{
    public partial class Servers
    {
        [Inject]
        private IConfigurableLoggerService _Logger { get; set; } = default!;
        [Inject]
        private APIService APIService { get; set; } = default!;
        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        private RadzenDataGrid<ServerInformationModel> ServerGrid = new();
        private List<ServerInformationModel> ServerRecords = [];

        private List<string> HostNames = [];
        private List<string> Games = [];
        private List<string> Connections = [];
        private List<string> Downtimes = [];
        private bool ShowModal;
        private string NewServerName = string.Empty;
        private string NewHostName = string.Empty;
        private string NewGame = string.Empty;
        private string NewConnection = string.Empty;
        private string NewDowntime = string.Empty;
        private int NewEventInterval;
        private bool IsLoading;
        private string ErrorMessage = string.Empty;

        /// <summary>
        /// Loads and transforms the data.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Opened Servers Page");

            ServerRecords = await APIService.GetServers();

            List<MachineModel> machines = await GetMachines();
            HostNames.AddRange(machines.Where(m => !m.IsDeleted)
                .Select(m => m.HostName));

            List<GameModel> games = await GetGames();
            Games.AddRange(games.Where(g => !g.IsDeleted)
                .Select(g => $"{g.Name} ({g.Version})"));

            List<ConnectionModel> connections = await GetConnections();
            Connections.AddRange(connections.Where(c => !c.IsDeleted)
                .Select(c => $"{c.IPAddress}:{c.Port}"));

            List<DowntimeModel> downtimes = await GetDowntimes();
            Downtimes.AddRange(downtimes.Where(d => !d.IsDeleted)
                .Select(d => $"{d.Time} ({d.Duration})"));
        }

        /// <summary>
        /// Loads the machine data.
        /// </summary>
        private async Task<List<MachineModel>> GetMachines()
        {
            List<MachineModel> machines = [];

            bool nextPage = true;
            int pageNumber = 1;

            while (nextPage)
            {
                PagedAPIResponseModel<MachineModel>? pagedMachines = await APIService.GetPagedConfiguration<PagedAPIResponseModel<MachineModel>?>("machine",
                    200,
                    pageNumber);

                if (pagedMachines != null && pagedMachines.EntryCount > 0)
                {
                    machines.AddRange(pagedMachines.Entries);

                    if (pageNumber < pagedMachines.TotalPageCount)
                    {
                        pageNumber++;
                    }

                    else
                    {
                        nextPage = false;
                    }
                }

                else
                {
                    nextPage = false;
                }
            }

            return machines;
        }

        /// <summary>
        /// Loads the game data.
        /// </summary>
        private async Task<List<GameModel>> GetGames()
        {
            List<GameModel> games = [];

            bool nextPage = true;
            int pageNumber = 1;

            while (nextPage)
            {
                PagedAPIResponseModel<GameModel>? pagedGames = await APIService.GetPagedConfiguration<PagedAPIResponseModel<GameModel>?>("game",
                    200,
                    pageNumber);

                if (pagedGames != null && pagedGames.EntryCount > 0)
                {
                    games.AddRange(pagedGames.Entries);

                    if (pageNumber < pagedGames.TotalPageCount)
                    {
                        pageNumber++;
                    }

                    else
                    {
                        nextPage = false;
                    }
                }

                else
                {
                    nextPage = false;
                }
            }

            return games;
        }

        /// <summary>
        /// Loads the connection data.
        /// </summary>
        private async Task<List<ConnectionModel>> GetConnections()
        {
            List<ConnectionModel> connections = [];

            bool nextPage = true;
            int pageNumber = 1;

            while (nextPage)
            {
                PagedAPIResponseModel<ConnectionModel>? pagedConnections = await APIService.GetPagedConfiguration<PagedAPIResponseModel<ConnectionModel>?>("connection",
                    200,
                    pageNumber);

                if (pagedConnections != null && pagedConnections.EntryCount > 0)
                {
                    connections.AddRange(pagedConnections.Entries);

                    if (pageNumber < pagedConnections.TotalPageCount)
                    {
                        pageNumber++;
                    }

                    else
                    {
                        nextPage = false;
                    }
                }

                else
                {
                    nextPage = false;
                }
            }

            return connections;
        }

        /// <summary>
        /// Loads the donwtime data.
        /// </summary>
        private async Task<List<DowntimeModel>> GetDowntimes()
        {
            List<DowntimeModel> downtimes = [];

            bool nextPage = true;
            int pageNumber = 1;

            while (nextPage)
            {
                PagedAPIResponseModel<DowntimeModel>? pagedDowntimes = await APIService.GetPagedConfiguration<PagedAPIResponseModel<DowntimeModel>?>("downtime",
                    200,
                    pageNumber);

                if (pagedDowntimes != null && pagedDowntimes.EntryCount > 0)
                {
                    downtimes.AddRange(pagedDowntimes.Entries);

                    if (pageNumber < pagedDowntimes.TotalPageCount)
                    {
                        pageNumber++;
                    }

                    else
                    {
                        nextPage = false;
                    }
                }

                else
                {
                    nextPage = false;
                }
            }

            return downtimes;
        }

        /// <summary>
        /// Shows the create server modal.
        /// </summary>
        private void OpenModal()
        {
            NewServerName = string.Empty;
            NewHostName = string.Empty;
            NewGame = string.Empty;
            NewConnection = string.Empty;
            NewDowntime = string.Empty;
            NewEventInterval = 0;
            ErrorMessage = string.Empty;
            ShowModal = true;

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Opened Create Server Modal");
        }

        /// <summary>
        /// Hides the create server model.
        /// </summary>
        private void CloseModal()
        {
            ErrorMessage = string.Empty;
            ShowModal = false;

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Closed Create Server Modal");
        }

        /// <summary>
        /// Directs the user to the edit page.
        /// </summary>
        private void NavigateToEdit(int id) => Navigation.NavigateTo($"/servers/{id}");

        /// <summary>
        /// Performs the server creation steps.
        /// </summary>
        private async Task CreateServer()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Create Server Clicked");

            IsLoading = true;
            bool success = false;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(NewServerName))
            {
                ErrorMessage = "Name is required.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                IsLoading = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(NewHostName))
            {
                ErrorMessage = "Host name is required.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                IsLoading = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(NewGame))
            {
                ErrorMessage = "Game is required.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                IsLoading = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(NewConnection))
            {
                ErrorMessage = "Connection is required.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                IsLoading = false;
                return;
            }

            if (NewEventInterval == 0)
            {
                ErrorMessage = "Event interval is required.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                IsLoading = false;
                return;
            }

            ServerInformationModel? existingServer = ServerRecords.Find(s => s.Name == NewServerName);

            if (existingServer == null)
            {
                string[] gameParts = NewGame.Split(' ');
                string version = gameParts[1].Replace("(", "")
                    .Replace(")", "");
                string[] connectionParts = NewConnection.Split(':');
                string[] downtimeParts = [];
                string? duration = null;

                if (!string.IsNullOrWhiteSpace(NewDowntime))
                {
                    downtimeParts = NewDowntime.Split(' ');
                    duration = downtimeParts[1].Replace("(", "")
                        .Replace(")", "");
                }

                ServerRequestModel server = new()
                {
                    Name = NewServerName,
                    EventInterval = NewEventInterval,
                    HostName = NewHostName,
                    Game = gameParts[0],
                    GameVersion = version,
                    IPAddress = connectionParts[0],
                    Port = int.Parse(connectionParts[1]),
                    Time = NewDowntime == string.Empty ? null : downtimeParts[0],
                    Duration = duration == null ? null : int.Parse(duration)
                };

                (ServerInformationModel? newServer, ResponseModel? apiResponse) = await APIService.CreateServer(server);

                if (newServer != null)
                {
                    ServerRecords.Add(newServer);
                    success = true;
                }

                else
                {
                    ErrorMessage = $"API returned {apiResponse?.StatusCode} ({apiResponse?.Message})";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                }
            }

            else
            {
                ErrorMessage = "A server with that name already exists.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                IsLoading = false;
                return;
            }

            await InvokeAsync(StateHasChanged);

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Create Success: {success}");

            await Task.Delay(2000).ContinueWith(_ =>
            {
                if (success)
                {
                    ShowModal = false;
                }

                ErrorMessage = string.Empty;

                ServerGrid.Reload();
                InvokeAsync(StateHasChanged);
            });

            IsLoading = false;
        }
    }
}
