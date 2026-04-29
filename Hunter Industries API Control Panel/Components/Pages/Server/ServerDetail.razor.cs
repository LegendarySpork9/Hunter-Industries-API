// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Models.Requests;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Services;
using Microsoft.AspNetCore.Components;

namespace HunterIndustriesAPIControlPanel.Components.Pages.Server
{
    public partial class ServerDetail
    {
        [Inject]
        private IConfigurableLoggerService _Logger { get; set; } = default!;

        [Inject]
        private APIService APIService { get; set; } = default!;

        [Parameter]
        public int Id { get; set; }

        private ServerInformationModel? Server;
        private ServerStatisticsModel? Statistics;

        private bool LoadingData = true;
        private HashSet<string> VisibleHealthLabels = [];
        private const int HealthLabelThreshold = 15;
        private const int HealthLabelStep = 5;
        private List<string> HostNames = [];
        private List<string> Games = [];
        private List<string> Connections = [];
        private List<string> Downtimes = [];
        private string EditServerName = string.Empty;
        private string EditHostName = string.Empty;
        private string EditGame = string.Empty;
        private string EditConnection = string.Empty;
        private string EditDowntime = string.Empty;
        private int EditEventInterval;
        private bool EditIsActive;
        private bool IsLoading;
        private string ErrorMessage = string.Empty;
        private bool SaveSuccess;
        private string[] ComponentAlertColours = [];
        private string[] StatusAlertColours = [];

        private static readonly string[] DefaultPalette = new[]
        {
            "#4472C4", "#ED7D31", "#A5A5A5", "#FFC000", "#5B9BD5",
            "#70AD47", "#264478", "#9B57A0", "#636363", "#EB7E30"
        };

        /// <summary>
        /// Sets the page title.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Opened Server Page");

            Server = await APIService.GetServer(Id);

            if (Server != null)
            {
                EditServerName = Server.Name;
                EditHostName = Server.HostName;
                EditGame = $"{Server.Game} ({Server.GameVersion})";
                EditConnection = $"{Server.Connection.IpAddress}:{Server.Connection.Port}";
                EditDowntime = Server.Downtime == null ? string.Empty : $"{Server.Downtime.Time} ({Server.Downtime.Duration})";
                EditEventInterval = Server.EventInterval;
                EditIsActive = Server.IsActive;
            }

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

            await LoadData();

            LoadingData = false;
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
                PagedAPIResponseModel<MachineModel>? pagedMachines = await APIService.GetMachines(200,
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
                PagedAPIResponseModel<GameModel>? pagedGames = await APIService.GetGames(200,
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
                PagedAPIResponseModel<ConnectionModel>? pagedConnections = await APIService.GetConnections(200,
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
                PagedAPIResponseModel<DowntimeModel>? pagedDowntimes = await APIService.GetDowntimes(200,
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
        /// Loads and transforms the data.
        /// </summary>
        private async Task LoadData()
        {
            Statistics = await APIService.GetServerStatistics(Id);

            if (Statistics != null)
            {
                int healthStep = Statistics.ServerHealth.Count > HealthLabelThreshold ? HealthLabelStep : 1;
                VisibleHealthLabels = [.. Statistics.ServerHealth.Where((_, i) => i % healthStep == 0)
                    .Select(t => t.Day)];

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Visible Health Label(s): {VisibleHealthLabels.Count}");

                ComponentAlertColours = [.. Statistics.ComponentAlerts.Select((_, ca) => DefaultPalette[ca % DefaultPalette.Length])];

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Component Alert Colour(s): {ComponentAlertColours.Length}");

                StatusAlertColours = [.. Statistics.StatusAlerts.Select(sa => sa.Status switch
                {
                    "Resolved" => "#28a745",
                    "Reported" => "#dc3545",
                    "Investigating" => "#ffc107",
                    _ => "#6c757d"
                })];

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Status Alert Colour(s): {StatusAlertColours.Length}");
            }
        }

        /// <summary>
        /// Returns the health day label or blank if the axis is being thinned out.
        /// </summary>
        private string FormatHealthDay(object value)
        {
            string label = string.Empty;

            if (value is string day && VisibleHealthLabels.Contains(day))
            {
                label = day;
            }

            return label;
        }

        /// <summary>
        /// Performs the server update steps.
        /// </summary>
        private async Task SaveServer()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Save Changes Clicked");

            IsLoading = true;

            if (Server != null)
            {
                ServerUpdateRequestModel server = new();

                if (!string.IsNullOrWhiteSpace(EditServerName) && EditServerName != Server.Name)
                {
                    server.Name = EditServerName;

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Name: {Server.Name} -> {EditServerName}");
                }

                if (EditEventInterval != 0 && EditEventInterval != Server.EventInterval)
                {
                    server.Duration = EditEventInterval;

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Event Interval: {Server.EventInterval} -> {EditEventInterval}");
                }

                if (!string.IsNullOrWhiteSpace(EditHostName) && EditHostName != Server.HostName)
                {
                    server.HostName = EditHostName;

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Host Name: {Server.HostName} -> {EditHostName}");
                }

                if (!string.IsNullOrWhiteSpace(EditGame))
                {
                    string[] gameParts = EditGame.Split(' ');
                    string version = gameParts[1].Replace("(", "")
                        .Replace(")", "");

                    if (gameParts[0] != Server.Game || version != Server.GameVersion)
                    {
                        server.Game = gameParts[0];
                        server.GameVersion = version;

                        if (gameParts[0] != Server.Game)
                        {
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Game: {Server.Game} -> {gameParts[0]}");
                        }

                        if (version != Server.GameVersion)
                        {
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Game Version: {Server.GameVersion} -> {version}");
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(EditConnection))
                {
                    string[] connectionParts = EditConnection.Split(':');

                    if (connectionParts[0] != Server.Connection.IpAddress || connectionParts[1] != Server.Connection.Port.ToString())
                    {
                        server.IPAddress = connectionParts[0];
                        server.Port = int.Parse(connectionParts[1]);

                        if (connectionParts[0] != Server.Connection.IpAddress)
                        {
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Ip Address: {Server.Connection.IpAddress} -> {connectionParts[0]}");
                        }

                        if (connectionParts[1] != Server.Connection.Port.ToString())
                        {
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Port: {Server.Connection.Port} -> {connectionParts[1]}");
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(EditDowntime))
                {
                    if (EditDowntime == "No Downtime")
                    {
                        server.ClearDowntime = true;

                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Downtime: {Server.Downtime?.Time} -> null");
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Downtime Duration: {Server.Downtime?.Duration} -> null");
                    }

                    else
                    {
                        string[] downtimeParts = EditDowntime.Split(' ');
                        string duration = downtimeParts[1].Replace("(", "")
                            .Replace(")", "");

                        if (Server.Downtime == null)
                        {
                            server.Time = downtimeParts[0];
                            server.Duration = int.Parse(duration);

                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Downtime: null -> {downtimeParts[0]}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Downtime Duration: null -> {duration}");
                        }

                        else
                        {
                            if (downtimeParts[0] != Server.Downtime.Time || duration != Server.Downtime.Duration.ToString())
                            {
                                server.Time = downtimeParts[0];
                                server.Duration = int.Parse(duration);

                                if (downtimeParts[0] != Server.Downtime.Time)
                                {
                                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Downtime: {Server.Downtime.Time} -> {downtimeParts[0]}");
                                }

                                else if (duration != Server.Downtime.Duration.ToString())
                                {
                                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Downtime Duration: {Server.Downtime.Duration} -> {duration}");
                                }
                            }
                        }
                    }
                }

                if (EditIsActive != Server.IsActive)
                {
                    server.IsActive = EditIsActive;

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Is Active: {Server.IsActive} -> {EditIsActive}");
                }

                (ServerInformationModel? updatedServer, ResponseModel? apiResponse) = await APIService.UpdateServer(Id,
                    server);

                if (updatedServer != null)
                {
                    SaveSuccess = true;
                    Server = updatedServer;
                    EditServerName = Server.Name;
                    EditHostName = Server.HostName;
                    EditGame = $"{Server.Game} ({Server.GameVersion})";
                    EditConnection = $"{Server.Connection.IpAddress}:{Server.Connection.Port}";
                    EditDowntime = Server.Downtime == null ? string.Empty : $"{Server.Downtime.Time} ({Server.Downtime.Duration})";
                    EditEventInterval = Server.EventInterval;
                    EditIsActive = Server.IsActive;
                }

                else
                {
                    SaveSuccess = false;
                    ErrorMessage = $"API returned {apiResponse?.StatusCode} ({apiResponse?.Message})";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                }

                await InvokeAsync(StateHasChanged);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Save Success: {SaveSuccess}");

                await Task.Delay(2000).ContinueWith(_ =>
                {
                    SaveSuccess = false;
                    ErrorMessage = string.Empty;

                    InvokeAsync(StateHasChanged);
                });
            }

            IsLoading = false;
        }
    }
}
