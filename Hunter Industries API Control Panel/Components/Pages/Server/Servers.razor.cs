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
        private bool IsEditing;
        private string ModalServerName = string.Empty;
        private string ModalHostName = string.Empty;
        private string ModalGame = string.Empty;
        private string ModalConnection = string.Empty;
        private string ModalDowntime = string.Empty;
        private int ModalEventInterval;
        private int EditServerId;
        private bool? EditIsActive;
        private bool IsLoading;
        private string ErrorMessage = string.Empty;
        private string SuccessMessage = string.Empty;

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
        /// Shows the create server modal.
        /// </summary>
        private void OpenCreateModal()
        {
            IsEditing = false;
            ModalServerName = string.Empty;
            ModalHostName = string.Empty;
            ModalGame = string.Empty;
            ModalConnection = string.Empty;
            ModalDowntime = string.Empty;
            ModalEventInterval = 0;
            EditIsActive = null;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
            ShowModal = true;

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Opened Create Server Modal");
        }

        /// <summary>
        /// Shows the edit server modal.
        /// </summary>
        private void ShowEditModal(ServerInformationModel server)
        {
            IsEditing = true;
            EditServerId = server.Id;
            ModalServerName = server.Name;
            ModalHostName = server.HostName;
            ModalGame = $"{server.Game} ({server.GameVersion})";
            ModalConnection = $"{server.Connection.IpAddress}:{server.Connection.Port}";
            ModalDowntime = server.Downtime == null ? string.Empty : $"{server.Downtime.Time} ({server.Downtime.Duration})";
            ModalEventInterval = server.EventInterval;
            EditIsActive = server.IsActive;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
            ShowModal = true;

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Opened Edit Server Modal");
        }

        /// <summary>
        /// Hides the create server model.
        /// </summary>
        private void CloseModal()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
            ShowModal = false;

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Closed Server Modal");
        }

        /// <summary>
        /// Performs the server creation/update steps.
        /// </summary>
        private async Task SaveServer()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Save Server Clicked");

            IsLoading = true;
            bool success = false;

            if (IsEditing)
            {
                success = await UpdateServer();

                await InvokeAsync(StateHasChanged);
            }

            else
            {
                success = await CreateServer();

                await InvokeAsync(StateHasChanged);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Save Success: {success}");

            if (!string.IsNullOrWhiteSpace(SuccessMessage) || !string.IsNullOrWhiteSpace(ErrorMessage))
            {
                await Task.Delay(2000).ContinueWith(_ =>
                {
                    if (success)
                    {
                        ShowModal = false;
                    }

                    SuccessMessage = string.Empty;
                    ErrorMessage = string.Empty;

                    ServerGrid.Reload();
                    InvokeAsync(StateHasChanged);
                });
            }

            IsLoading = false;
        }

        /// <summary>
        /// Performs the server update steps.
        /// </summary>
        private async Task<bool> UpdateServer()
        {
            bool success = false;

            ServerUpdateRequestModel server = new();

            ServerInformationModel existingServer = ServerRecords.First(s => s.Id == EditServerId);

            if (!string.IsNullOrWhiteSpace(ModalServerName) && ModalServerName != existingServer.Name)
            {
                server.Name = ModalServerName;

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Name: {existingServer.Name} -> {ModalServerName}");
            }

            if (ModalEventInterval != 0 && ModalEventInterval != existingServer.EventInterval)
            {
                server.Duration = ModalEventInterval;

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Event Interval: {existingServer.EventInterval} -> {ModalEventInterval}");
            }

            if (!string.IsNullOrWhiteSpace(ModalHostName) && ModalHostName != existingServer.HostName)
            {
                server.HostName = ModalHostName;

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Host Name: {existingServer.HostName} -> {ModalHostName}");
            }

            if (!string.IsNullOrWhiteSpace(ModalGame))
            {
                string[] gameParts = ModalGame.Split(' ');
                string version = gameParts[1].Replace("(", "")
                    .Replace(")", "");

                if (gameParts[0] != existingServer.Game)
                {
                    server.Game = gameParts[0];

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Game: {existingServer.Game} -> {gameParts[0]}");
                }

                if (version != existingServer.GameVersion)
                {
                    server.GameVersion = version;

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Game Version: {existingServer.GameVersion} -> {version}");
                }
            }

            if (!string.IsNullOrWhiteSpace(ModalConnection))
            {
                string[] connectionParts = ModalConnection.Split(':');

                if (connectionParts[0] != existingServer.Connection.IpAddress)
                {
                    server.IPAddress = connectionParts[0];

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Ip Address: {existingServer.Connection.IpAddress} -> {connectionParts[0]}");
                }

                if (connectionParts[1] != existingServer.Connection.Port.ToString())
                {
                    server.Port = int.Parse(connectionParts[1]);

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Port: {existingServer.Connection.Port} -> {connectionParts[1]}");
                }
            }

            if (!string.IsNullOrWhiteSpace(ModalDowntime))
            {
                string[] downtimeParts = ModalDowntime.Split(' ');
                string duration = downtimeParts[1].Replace("(", "")
                    .Replace(")", "");

                if (existingServer.Downtime == null)
                {
                    server.Time = downtimeParts[0];
                    server.Duration = int.Parse(duration);

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Downtime: null -> {downtimeParts[0]}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Downtime Duration: null -> {duration}");
                }

                else if (downtimeParts[0] != existingServer.Downtime.Time)
                {
                    server.Time = downtimeParts[0];

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Downtime: {existingServer.Downtime.Time} -> {downtimeParts[0]}");
                }

                else if (duration != existingServer.Downtime.Duration.ToString())
                {
                    server.Duration = int.Parse(duration);

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Downtime Duration: {existingServer.Downtime.Duration} -> {duration}");
                }
            }

            if (EditIsActive.HasValue && EditIsActive != existingServer.IsActive)
            {
                server.IsActive = EditIsActive.Value;

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Server Is Active: {existingServer.IsActive} -> {EditIsActive.Value}");
            }

            ServerInformationModel? updatedServer = await APIService.UpdateServer(EditServerId,
                server);

            if (updatedServer != null)
            {
                int index = ServerRecords.FindIndex(s => s.Id == EditServerId);
                ServerRecords[index] = updatedServer;
                SuccessMessage = "Saved changes successfully!";
                success = true;
            }

            else
            {
                ErrorMessage = "Something went wrong. Please check logs for details.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
            }

            return success;
        }

        /// <summary>
        /// Performs the server creation steps.
        /// </summary>
        private async Task<bool> CreateServer()
        {
            bool success = false;

            if (string.IsNullOrWhiteSpace(ModalServerName))
            {
                ErrorMessage = "Name is required.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                return success;
            }

            if (string.IsNullOrWhiteSpace(ModalHostName))
            {
                ErrorMessage = "Host name is required.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                return success;
            }

            if (string.IsNullOrWhiteSpace(ModalGame))
            {
                ErrorMessage = "Game is required.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                return success;
            }

            if (string.IsNullOrWhiteSpace(ModalConnection))
            {
                ErrorMessage = "Connection is required.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                return success;
            }

            if (ModalEventInterval == 0)
            {
                ErrorMessage = "Event interval is required.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                return success;
            }

            ServerInformationModel? existingServer = ServerRecords.Find(s => s.Name == ModalServerName);

            if (existingServer == null)
            {
                string[] gameParts = ModalGame.Split(' ');
                string version = gameParts[1].Replace("(", "")
                    .Replace(")", "");
                string[] connectionParts = ModalConnection.Split(':');
                string[] downtimeParts = [];
                string? duration = null;

                if (!string.IsNullOrWhiteSpace(ModalDowntime))
                {
                    downtimeParts = ModalDowntime.Split(' ');
                    duration = downtimeParts[1].Replace("(", "")
                        .Replace(")", "");
                }

                ServerRequestModel server = new()
                {
                    Name = ModalServerName,
                    EventInterval = ModalEventInterval,
                    HostName = ModalHostName,
                    Game = gameParts[0],
                    GameVersion = version,
                    IPAddress = connectionParts[0],
                    Port = int.Parse(connectionParts[1]),
                    Time = ModalDowntime == string.Empty ? null : downtimeParts[0],
                    Duration = duration == null ? null : int.Parse(duration)
                };

                ServerInformationModel? newServer = await APIService.CreateServer(server);

                if (newServer != null)
                {
                    ServerRecords.Add(newServer);
                    SuccessMessage = "Created server successfully!";
                    success = true;
                }

                else
                {
                    ErrorMessage = "Something went wrong. Please check logs for details.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                }
            }

            else
            {
                ErrorMessage = "A server with that name already exists.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
            }

            return success;
        }
    }
}
