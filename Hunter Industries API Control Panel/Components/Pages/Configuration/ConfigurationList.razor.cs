// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Functions;
using HunterIndustriesAPIControlPanel.Mappers;
using HunterIndustriesAPIControlPanel.Models;
using HunterIndustriesAPIControlPanel.Models.Requests.Post;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;
using HunterIndustriesAPIControlPanel.Services;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace HunterIndustriesAPIControlPanel.Components.Pages.Configuration
{
    public partial class ConfigurationList
    {
        [Inject]
        private IConfigurableLoggerService _Logger { get; set; } = default!;
        [Inject]
        private IFileSystem _FileSystem { get; set; } = default!;
        [Inject]
        private NavigationManager Navigation { get; set; } = default!;
        [Inject]
        private APIService APIService { get; set; } = default!;
        [Inject]
        private APISettingsModel APISettings { get; set; } = default!;

        [SupplyParameterFromQuery(Name = "page")]
        public int? QueryPage { get; set; }

        [Parameter]
        public string Entity { get; set; } = string.Empty;

        private PaginatedResponse<ConfigurationListObjectModel>? Records;

        private string DisplayName = string.Empty;
        private bool ShowCreateModal;
        private List<string> Phrases = [];
        private string ControlPanelApplication = string.Empty;
        private string NewAppName = string.Empty;
        private string NewAppPhrase = string.Empty;
        private string NewAuthPhrase = string.Empty;
        private string NewComponentName = string.Empty;
        private string NewConnectionIP = string.Empty;
        private int NewConnectionPort = 0;
        private string NewDowntimeTime = string.Empty;
        private int NewDowntimeDuration = 0;
        private string NewGameName = string.Empty;
        private string NewGameVersion = string.Empty;
        private string NewMachineHostName = string.Empty;
        private bool IsLoading;
        private string ErrorMessage = string.Empty;
        private bool ShowDeleteConfirm;
        private ConfigurationListObjectModel? EntityToDelete;
        private int PageSize = 25;
        private int PageNumber = 1;

        private static readonly Dictionary<string, string> DisplayNames = new()
        {
            ["application"] = "Application",
            ["authorisation"] = "Authorisation",
            ["component"] = "Component",
            ["connection"] = "Connection",
            ["downtime"] = "Downtime",
            ["game"] = "Game",
            ["machine"] = "Machine"
        };

        /// <summary>
        /// Loads and transforms the data.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Opened Configuration List Page");

            if (QueryPage.HasValue && QueryPage.Value > 0)
            {
                PageNumber = QueryPage.Value;
            }

            DisplayName = DisplayNames.GetValueOrDefault(Entity, Entity);

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Entity: {Entity}");
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Page Number: {PageNumber}");
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Display Name: {DisplayName}");

            await LoadData();
        }

        /// <summary>
        /// Loads and transforms the configuration data.
        /// </summary>
        private async Task LoadData()
        {
            ConfigurationFunction _configurationFunction = new(_FileSystem,
                APISettings);

            if (Entity == "application")
            {
                PagedAPIResponseModel<ApplicationModel>? applications = await APIService.GetPagedConfiguration<PagedAPIResponseModel<ApplicationModel>?>(Entity,
                    PageSize,
                    PageNumber);

                if (applications != null)
                {
                    Records = new()
                    {
                        Entries = [.. applications.Entries.Select(app => app.ToListObject())],
                        EntryCount = applications.EntryCount,
                        PageNumber = applications.PageNumber,
                        PageSize = applications.PageSize,
                        TotalPageCount = applications.TotalPageCount,
                        TotalCount = applications.TotalCount
                    };
                    ControlPanelApplication = _configurationFunction.GetControlPanelApplication(applications.Entries);
                }

                PageNumber = Records?.PageNumber ?? PageNumber;
                Phrases = await GetPhrases();
            }

            else if (Entity == "authorisation")
            {
                PagedAPIResponseModel<AuthorisationModel>? authorisations = await APIService.GetPagedConfiguration<PagedAPIResponseModel<AuthorisationModel>?>(Entity,
                    PageSize,
                    PageNumber);

                if (authorisations != null)
                {
                    Records = new()
                    {
                        Entries = [.. authorisations.Entries.Select(a => a.ToListObject())],
                        EntryCount = authorisations.EntryCount,
                        PageNumber = authorisations.PageNumber,
                        PageSize = authorisations.PageSize,
                        TotalPageCount = authorisations.TotalPageCount,
                        TotalCount = authorisations.TotalCount
                    };
                }
            }

            else if (Entity == "component")
            {
                PagedAPIResponseModel<ComponentModel>? components = await APIService.GetPagedConfiguration<PagedAPIResponseModel<ComponentModel>?>(Entity,
                    PageSize,
                    PageNumber);

                if (components != null)
                {
                    Records = new()
                    {
                        Entries = [.. components.Entries.Select(com => com.ToListObject())],
                        EntryCount = components.EntryCount,
                        PageNumber = components.PageNumber,
                        PageSize = components.PageSize,
                        TotalPageCount = components.TotalPageCount,
                        TotalCount = components.TotalCount
                    };
                }
            }

            else if (Entity == "connection")
            {
                PagedAPIResponseModel<ConnectionModel>? connections = await APIService.GetPagedConfiguration<PagedAPIResponseModel<ConnectionModel>?>(Entity,
                    PageSize,
                    PageNumber);

                if (connections != null)
                {
                    Records = new()
                    {
                        Entries = [.. connections.Entries.Select(c => c.ToListObject())],
                        EntryCount = connections.EntryCount,
                        PageNumber = connections.PageNumber,
                        PageSize = connections.PageSize,
                        TotalPageCount = connections.TotalPageCount,
                        TotalCount = connections.TotalCount
                    };
                }
            }

            else if (Entity == "downtime")
            {
                PagedAPIResponseModel<DowntimeModel>? downtimes = await APIService.GetPagedConfiguration<PagedAPIResponseModel<DowntimeModel>?>(Entity,
                    PageSize,
                    PageNumber);

                if (downtimes != null)
                {
                    Records = new()
                    {
                        Entries = [.. downtimes.Entries.Select(d => d.ToListObject())],
                        EntryCount = downtimes.EntryCount,
                        PageNumber = downtimes.PageNumber,
                        PageSize = downtimes.PageSize,
                        TotalPageCount = downtimes.TotalPageCount,
                        TotalCount = downtimes.TotalCount
                    };
                }
            }

            else if (Entity == "game")
            {
                PagedAPIResponseModel<GameModel>? games = await APIService.GetPagedConfiguration<PagedAPIResponseModel<GameModel>?>(Entity,
                    PageSize,
                    PageNumber);

                if (games != null)
                {
                    Records = new()
                    {
                        Entries = [.. games.Entries.Select(g => g.ToListObject())],
                        EntryCount = games.EntryCount,
                        PageNumber = games.PageNumber,
                        PageSize = games.PageSize,
                        TotalPageCount = games.TotalPageCount,
                        TotalCount = games.TotalCount
                    };
                }
            }

            else if (Entity == "machine")
            {
                PagedAPIResponseModel<MachineModel>? machines = await APIService.GetPagedConfiguration<PagedAPIResponseModel<MachineModel>?>(Entity,
                    PageSize,
                    PageNumber);

                if (machines != null)
                {
                    Records = new()
                    {
                        Entries = [.. machines.Entries.Select(m => m.ToListObject())],
                        EntryCount = machines.EntryCount,
                        PageNumber = machines.PageNumber,
                        PageSize = machines.PageSize,
                        TotalPageCount = machines.TotalPageCount,
                        TotalCount = machines.TotalCount
                    };
                }
            }

            if (Records == null)
            {
                Records = new()
                {
                    Entries = [],
                    EntryCount = 0,
                    PageNumber = 1,
                    PageSize = 25,
                    TotalPageCount = 0,
                    TotalCount = 0
                };
            }

            UpdateUrl();
        }

        /// <summary>
        /// Loads the authorisation data.
        /// </summary>
        private async Task<List<string>> GetPhrases()
        {
            List<string> phrases = [];

            bool nextPage = true;
            int pageNumber = 1;

            while (nextPage)
            {
                PagedAPIResponseModel<AuthorisationModel>? pagedAuthorisation = await APIService.GetPagedConfiguration<PagedAPIResponseModel<AuthorisationModel>?>("authorisation",
                    200,
                    pageNumber,
                    false);

                if (pagedAuthorisation != null && pagedAuthorisation.EntryCount > 0)
                {
                    phrases.AddRange(pagedAuthorisation.Entries.Select(a => a.Phrase));

                    if (pageNumber < pagedAuthorisation.TotalPageCount)
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

            return phrases;
        }

        /// <summary>
        /// Updates the page url.
        /// </summary>
        private void UpdateUrl() => Navigation.NavigateTo($"/configuration/{Entity}?page={PageNumber}", replace: true);

        /// <summary>
        /// Starts adding a new configuration object.
        /// </summary>
        private void OpenCreateModal()
        {
            NewAppName = string.Empty;
            NewAppPhrase = string.Empty;
            NewAuthPhrase = string.Empty;
            NewComponentName = string.Empty;
            NewConnectionIP = string.Empty;
            NewConnectionPort = 0;
            NewDowntimeTime = string.Empty;
            NewDowntimeDuration = 0;
            NewGameName = string.Empty;
            NewGameVersion = string.Empty;
            NewMachineHostName = string.Empty;
            ErrorMessage = string.Empty;
            ShowCreateModal = true;

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Opened Create Entity Modal");
        }

        /// <summary>
        /// Cancels the new configuration object.
        /// </summary>
        private void CloseCreateModal()
        {
            ShowCreateModal = false;

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Closed Create Entity Modal");
        }

        /// <summary>
        /// Adds the new configuration object.
        /// </summary>
        private async Task CreateEntity()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Create Clicked");

            IsLoading = true;
            bool success = false;
            ErrorMessage = string.Empty;
            ResponseModel? apiResponse = null;

            if (Entity == "application")
            {
                if (string.IsNullOrWhiteSpace(NewAppName) || string.IsNullOrWhiteSpace(NewAppPhrase))
                {
                    ErrorMessage = "Name and phrase are required.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                    IsLoading = false;
                    return;
                }

                ApplicationRequestModel application = new()
                {
                    Name = NewAppName,
                    Phrase = NewAppPhrase
                };

                (ApplicationModel? newApplication, apiResponse) = await APIService.CreateConfigurationEntity<ApplicationModel, ApplicationRequestModel>(Entity,
                    NewAppName,
                    application);

                success = newApplication != null;
            }

            else if (Entity == "authorisation")
            {
                if (string.IsNullOrWhiteSpace(NewAuthPhrase))
                {
                    ErrorMessage = "Phrase required.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                    IsLoading = false;
                    return;
                }

                AuthorisationRequestModel authorisation = new()
                {
                    Phrase = NewAuthPhrase
                };

                (AuthorisationModel? newAuthorisation, apiResponse) = await APIService.CreateConfigurationEntity<AuthorisationModel, AuthorisationRequestModel>(Entity,
                    NewAuthPhrase,
                    authorisation);

                success = newAuthorisation != null;
            }

            else if (Entity == "component")
            {
                if (string.IsNullOrWhiteSpace(NewComponentName))
                {
                    ErrorMessage = "Name is required.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                    IsLoading = false;
                    return;
                }

                ComponentRequestModel component = new()
                {
                    Name = NewComponentName
                };

                (ComponentModel? newComponent, apiResponse) = await APIService.CreateConfigurationEntity<ComponentModel, ComponentRequestModel>(Entity,
                    NewComponentName,
                    component);

                success = newComponent != null;
            }

            else if (Entity == "connection")
            {
                if (string.IsNullOrWhiteSpace(NewConnectionIP) || NewConnectionPort == 0)
                {
                    ErrorMessage = "Ip address and port are required.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                    IsLoading = false;
                    return;
                }

                ConnectionRequestModel connection = new()
                {
                    IPAddress = NewConnectionIP,
                    Port = NewConnectionPort
                };

                (ConnectionModel? newConnection, apiResponse) = await APIService.CreateConfigurationEntity<ConnectionModel, ConnectionRequestModel>(Entity,
                    $"{NewConnectionIP}:{NewConnectionPort}",
                    connection);

                success = newConnection != null;
            }

            else if (Entity == "downtime")
            {
                if (string.IsNullOrWhiteSpace(NewDowntimeTime) || NewDowntimeDuration == 0)
                {
                    ErrorMessage = "Time and duration are required.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                    IsLoading = false;
                    return;
                }

                DowntimeRequestModel downtime = new()
                {
                    Time = NewDowntimeTime,
                    Duration = NewDowntimeDuration
                };

                (DowntimeModel? newDowntime, apiResponse) = await APIService.CreateConfigurationEntity<DowntimeModel, DowntimeRequestModel>(Entity,
                    $"{NewDowntimeTime} ({NewDowntimeDuration})",
                    downtime);

                success = newDowntime != null;
            }

            else if (Entity == "game")
            {
                if (string.IsNullOrWhiteSpace(NewGameName) || string.IsNullOrWhiteSpace(NewGameVersion))
                {
                    ErrorMessage = "Game and game version are required.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                    IsLoading = false;
                    return;
                }

                GameRequestModel game = new()
                {
                    Name = NewGameName,
                    Version = NewGameVersion
                };

                (GameModel? newGame, apiResponse) = await APIService.CreateConfigurationEntity<GameModel, GameRequestModel>(Entity,
                    $"{NewGameName} ({NewGameVersion})",
                    game);

                success = newGame != null;
            }

            else if (Entity == "machine")
            {
                if (string.IsNullOrWhiteSpace(NewMachineHostName))
                {
                    ErrorMessage = "Host name is required.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                    IsLoading = false;
                    return;
                }

                MachineRequestModel machine = new()
                {
                    HostName = NewMachineHostName
                };

                (MachineModel? newMachine, apiResponse) = await APIService.CreateConfigurationEntity<MachineModel, MachineRequestModel>(Entity,
                    NewMachineHostName,
                    machine);

                success = newMachine != null;
            }

            if (apiResponse != null)
            {
                ErrorMessage = $"API returned {apiResponse?.StatusCode} ({apiResponse?.Message})";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
            }

            await InvokeAsync(StateHasChanged);

            await Task.Delay(2000).ContinueWith(_ =>
            {
                if (success)
                {
                    ShowCreateModal = false;
                }

                InvokeAsync(StateHasChanged);
            });

            await LoadData();
            ErrorMessage = string.Empty;
            IsLoading = false;
        }

        /// <summary>
        /// Directs the user to the edit page.
        /// </summary>
        private void NavigateToEdit(int id) => Navigation.NavigateTo($"/configuration/{Entity}/{id}?fromPage={PageNumber}");

        /// <summary>
        /// Directs the user to the logs page.
        /// </summary>
        private void NavigateToLogs(int id) => Navigation.NavigateTo($"/logs?application={id}");

        /// <summary>
        /// Shows the delete confirmation modal.
        /// </summary>
        private void ConfirmDelete(ConfigurationListObjectModel entity)
        {
            EntityToDelete = entity;
            ShowDeleteConfirm = true;

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Opened Delete Confirmation Modal");
        }

        /// <summary>
        /// Performs the configuration deletion steps.
        /// </summary>
        private async Task DeleteConfiguration()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Delete Configuration Clicked");

            if (EntityToDelete != null)
            {
                bool deleted = await APIService.DeleteConfigurationEntity(Entity,
                    EntityToDelete.Id);

                if (deleted && Records != null)
                {
                    int index = Records.Entries.IndexOf(EntityToDelete);
                    Records.Entries[index].IsDeleted = true;
                }
            }

            ShowDeleteConfirm = false;
            EntityToDelete = null;
        }

        /// <summary>
        /// Hides the delete confirmation modal.
        /// </summary>
        private void CancelDelete()
        {
            ShowDeleteConfirm = false;
            EntityToDelete = null;

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Closed Delete Confirmation Model");
        }

        /// <summary>
        /// Applys the set filters.
        /// </summary>
        private async Task ApplyFilters()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Apply Clicked");

            PageNumber = 1;
            await LoadData();
        }

        /// <summary>
        /// Loads the last page of audit logs.
        /// </summary>
        private async Task PreviousPage()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "<< Prev Clicked");

            if (PageNumber > 1)
            {
                PageNumber--;
                await LoadData();
            }
        }

        /// <summary>
        /// Loads the next page of audit logs.
        /// </summary>
        private async Task NextPage()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Next >> Clicked");

            if (PageNumber < Records?.TotalPageCount)
            {
                PageNumber++;
                await LoadData();
            }
        }
    }
}
