// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Functions;
using HunterIndustriesAPIControlPanel.Models;
using HunterIndustriesAPIControlPanel.Models.Requests.Patch;
using HunterIndustriesAPIControlPanel.Models.Requests.Post;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Services;
using Microsoft.AspNetCore.Components;

namespace HunterIndustriesAPIControlPanel.Components.Pages.Configuration
{
    public partial class ConfigurationDetail
    {
        [Inject]
        private IConfigurableLoggerService _Logger { get; set; } = default!;
        [Inject]
        private IFileSystem _FileSystem { get; set; } = default!;
        [Inject]
        private APIService APIService { get; set; } = default!;
        [Inject]
        private APISettingsModel APISettings { get; set; } = default!;

        [Parameter]
        public string Entity { get; set; } = string.Empty;
        [Parameter]
        public int Id { get; set; }

        private ApplicationModel? Application;
        private AuthorisationModel? Authorisation;
        private ComponentModel? Component;
        private ConnectionModel? Connection;
        private DowntimeModel? Downtime;
        private GameModel? Game;
        private MachineModel? Machine;

        private bool IsLoading;
        private bool HasData = false;
        private bool IsControlPanelApplication;
        private bool IsControlPanelAuthorisation;
        private bool IsDeleted;
        private bool IsSaving;
        private bool IsAddingSetting;
        private bool IsSavingSetting;
        private bool SaveSuccess;
        private bool AddSettingSuccess;
        private bool SaveSettingSuccess;
        private bool ShowDeleteConfirm;

        private string ErrorFor = string.Empty;
        private string ErrorMessage = string.Empty;

        private string DisplayName = string.Empty;
        private List<string> Phrases = [];
        private List<ApplicationSettingModel> UnchangedSettings = [];
        private string EditAppName = string.Empty;
        private string EditAppPhrase = string.Empty;
        private string DeleteDisplayName = string.Empty;
        private ApplicationSettingRequestModel? NewSetting;
        private int EditSettingId = 0;
        private int? DeleteSettingId = null;
        private string EditAuthPhrase = string.Empty;
        private string EditComponentName = string.Empty;
        private string EditConnectionIP = string.Empty;
        private int EditConnectionPort = 0;
        private string EditDowntimeTime = string.Empty;
        private int EditDowntimeDuration = 0;
        private string EditGameName = string.Empty;
        private string EditGameVersion = string.Empty;
        private string EditHostName = string.Empty;

        private List<KeyValuePair<string, string>> DataTypes =
        [
            new("Boolean", "Boolean"),
            new("Byte", "Byte (0 -> 255)"),
            new("Char", "Char"),
            new("DateOnly", "DateOnly"),
            new("DateTime", "DateTime"),
            new("DateTimeOffset", "DateTimeOffset"),
            new("Decimal", "Currancy"),
            new("Double", "Math Integer"),
            new("Guid", "Guid"),
            new("Int16", "Integer (~32 KB)"),
            new("Int32", "Integer (~2 GB)"),
            new("Int64", "Integer (~8 EB)"),
            new("Int128", "Integer (~139 BB)"),
            new("SByte", "SByte (-128 -> 127)"),
            new("Single", "Float"),
            new("String", "String"),
            new("TimeOnly", "TimeOnly"),
            new("TimeSpan", "Duration"),
            new("Uri", "Uri")
        ];
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
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                "Opened Configuration Detail Page");

            IsLoading = true;

            DisplayName = DisplayNames.GetValueOrDefault(
                Entity,
                Entity);

            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"Entity: {Entity}");
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"Display Name: {DisplayName}");

            await LoadData();

            if (Application != null || Authorisation != null || Component != null || Connection != null
                || Component != null || Downtime != null || Game != null || Machine != null)
            {
                HasData = true;
            }

            IsLoading = false;
        }

        /// <summary>
        /// Loads and transforms the configuration data.
        /// </summary>
        private async Task LoadData()
        {
            ConfigurationFunction _configurationFunction = new(
                _FileSystem,
                APISettings);

            if (Entity == "application")
            {
                Application = await APIService.GetConfigurationEntity<ApplicationModel?>(
                    Entity,
                    Id);

                if (Application != null)
                {
                    EditAppName = Application.Name;
                    EditAppPhrase = Application.Authorisation.Phrase;
                    UnchangedSettings = [.. Application.Settings.Select(s => new ApplicationSettingModel
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Type = s.Type,
                        Required = s.Required,
                        IsDeleted = s.IsDeleted
                    })];
                    IsControlPanelApplication = _configurationFunction.IsControlPanelApplication(Application);
                }

                Phrases = await GetPhrases();
            }

            else if (Entity == "authorisation")
            {
                Authorisation = await APIService.GetConfigurationEntity<AuthorisationModel?>(
                    Entity,
                    Id);

                if (Authorisation != null)
                {
                    EditAuthPhrase = Authorisation.Phrase;
                    IsControlPanelAuthorisation = _configurationFunction.IsControlPanelAuthorisation(Authorisation.Phrase);
                }
            }

            else if (Entity == "component")
            {
                Component = await APIService.GetConfigurationEntity<ComponentModel?>(
                    Entity,
                    Id);

                if (Component != null)
                {
                    EditComponentName = Component.Name;
                }
            }

            else if (Entity == "connection")
            {
                Connection = await APIService.GetConfigurationEntity<ConnectionModel?>(
                    Entity,
                    Id);

                if (Connection != null)
                {
                    EditConnectionIP = Connection.IPAddress;
                    EditConnectionPort = Connection.Port;
                }
            }

            else if (Entity == "downtime")
            {
                Downtime = await APIService.GetConfigurationEntity<DowntimeModel?>(
                    Entity,
                    Id);

                if (Downtime != null)
                {
                    EditDowntimeTime = Downtime.Time;
                    EditDowntimeDuration = Downtime.Duration;
                }
            }

            else if (Entity == "game")
            {
                Game = await APIService.GetConfigurationEntity<GameModel?>(
                    Entity,
                    Id);

                if (Game != null)
                {
                    EditGameName = Game.Name;
                    EditGameVersion = Game.Version;
                }
            }

            else if (Entity == "machine")
            {
                Machine = await APIService.GetConfigurationEntity<MachineModel?>(
                    Entity,
                    Id);

                if (Machine != null)
                {
                    EditHostName = Machine.HostName;
                }
            }
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
                PagedAPIResponseModel<AuthorisationModel>? pagedAuthorisation = await APIService.GetPagedConfiguration<PagedAPIResponseModel<AuthorisationModel>?>(
                    "authorisation",
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
        /// Shows the delete confirmation modal.
        /// </summary>
        private void ConfirmDelete(int? settingId = null)
        {
            if (settingId.HasValue)
            {
                DeleteSettingId = settingId.Value;
                DeleteDisplayName = "Application Setting";
            }

            else
            {
                DeleteDisplayName = DisplayName;
            }

            ShowDeleteConfirm = true;

            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                "Opened Delete Confirmation Modal");
        }

        /// <summary>
        /// Performs the configuration deletion steps.
        /// </summary>
        private async Task DeleteConfiguration()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                "Delete Clicked");

            if (DeleteSettingId.HasValue && Application != null)
            {
                bool deleted = await APIService.DeleteConfigurationEntity(
                    "applicationSetting",
                    DeleteSettingId.Value);
                
                int index = Application.Settings.FindIndex(s => s.Id == DeleteSettingId);
                Application.Settings[index].IsDeleted = deleted;
            }

            else if (Application != null)
            {
                bool deleted = await APIService.DeleteConfigurationEntity(
                    Entity,
                    Id);
                Application.IsDeleted = deleted;
                IsDeleted = deleted;
            }

            else if (Authorisation != null)
            {
                bool deleted = await APIService.DeleteConfigurationEntity(
                    Entity,
                    Id);
                Authorisation.IsDeleted = deleted;
                IsDeleted = deleted;
            }

            else if (Component != null)
            {
                bool deleted = await APIService.DeleteConfigurationEntity(
                    Entity,
                    Id);
                Component.IsDeleted = deleted;
                IsDeleted = deleted;
            }

            else if (Connection != null)
            {
                bool deleted = await APIService.DeleteConfigurationEntity(
                    Entity,
                    Id);
                Connection.IsDeleted = deleted;
                IsDeleted = deleted;
            }

            else if (Downtime != null)
            {
                bool deleted = await APIService.DeleteConfigurationEntity(
                    Entity,
                    Id);
                Downtime.IsDeleted = deleted;
                IsDeleted = deleted;
            }

            else if (Game != null)
            {
                bool deleted = await APIService.DeleteConfigurationEntity(
                    Entity,
                    Id);
                Game.IsDeleted = deleted;
                IsDeleted = deleted;
            }

            else if (Machine != null)
            {
                bool deleted = await APIService.DeleteConfigurationEntity(
                    Entity,
                    Id);
                Machine.IsDeleted = deleted;
                IsDeleted = deleted;
            }

            ShowDeleteConfirm = false;
        }

        /// <summary>
        /// Hides the delete confirmation modal.
        /// </summary>
        private void CancelDelete()
        {
            if (DeleteSettingId.HasValue)
            {
                DeleteSettingId = null;
            }

            DeleteDisplayName = string.Empty;
            ShowDeleteConfirm = false;

            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                "Closed Delete Confirmation Model");
        }

        /// <summary>
        /// Performs the application update steps.
        /// </summary>
        private async Task SaveApplication()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                "Save Application Clicked");

            IsSaving = true;

            if (Application != null)
            {
                ApplicationUpdateRequestModel applicationUpdate = new();

                if (!string.IsNullOrWhiteSpace(EditAppName) && EditAppName != Application.Name)
                {
                    applicationUpdate.Name = EditAppName;

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Application Name: {Application.Name} -> {EditAppName}");
                }

                if (!string.IsNullOrWhiteSpace(EditAppPhrase) && EditAppPhrase != Application.Authorisation.Phrase)
                {
                    applicationUpdate.Phrase = EditAppPhrase;

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Application Name: {Application.Authorisation.Phrase} -> {EditAppPhrase}");
                }

                (ApplicationModel? application, ResponseModel? apiResponse) = await APIService.UpdateConfigurationEntity<ApplicationModel, ApplicationUpdateRequestModel>(
                    Entity,
                    Id,
                    applicationUpdate);

                if (application != null)
                {
                    SaveSuccess = true;
                    Application = application;
                    EditAppName = Application.Name;
                    EditAppPhrase = Application.Authorisation.Phrase;
                }

                else
                {
                    SaveSuccess = false;
                    ErrorFor = "Entity";
                    ErrorMessage = $"API returned {apiResponse?.StatusCode} ({apiResponse?.Message})";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        ErrorMessage);
                }

                await InvokeAsync(StateHasChanged);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Save Success: {SaveSuccess}");

                await Task.Delay(2000).ContinueWith(_ =>
                {
                    SaveSuccess = false;
                    ErrorFor = string.Empty;
                    ErrorMessage = string.Empty;

                    InvokeAsync(StateHasChanged);
                });
            }

            IsSaving = false;
        }

        /// <summary>
        /// Performs the authorisation update steps.
        /// </summary>
        /// <returns></returns>
        private async Task SaveAuthorisation()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                "Save Clicked");

            IsSaving = true;

            if (Authorisation != null)
            {
                AuthorisationUpdateRequestModel authorisationUpdate = new();

                if (!string.IsNullOrWhiteSpace(EditAuthPhrase) && EditAuthPhrase != Authorisation.Phrase)
                {
                    authorisationUpdate.Phrase = EditAuthPhrase;

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Authorisation Phrase: {Authorisation.Phrase} -> {EditAuthPhrase}");
                }

                (AuthorisationModel? authorisation, ResponseModel? apiResponse) = await APIService.UpdateConfigurationEntity<AuthorisationModel, AuthorisationUpdateRequestModel>(
                    Entity,
                    Id,
                    authorisationUpdate);

                if (authorisation != null)
                {
                    SaveSuccess = true;
                    Authorisation = authorisation;
                    EditAuthPhrase = Authorisation.Phrase;
                }

                else
                {
                    SaveSuccess = false;
                    ErrorFor = "Entity";
                    ErrorMessage = $"API returned {apiResponse?.StatusCode} ({apiResponse?.Message})";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        ErrorMessage);
                }

                await InvokeAsync(StateHasChanged);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Save Success: {SaveSuccess}");

                await Task.Delay(2000).ContinueWith(_ =>
                {
                    SaveSuccess = false;
                    ErrorFor = string.Empty;
                    ErrorMessage = string.Empty;

                    InvokeAsync(StateHasChanged);
                });
            }

            IsSaving = false;
        }

        /// <summary>
        /// Performs the component update steps.
        /// </summary>
        private async Task SaveComponent()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                "Save Clicked");

            IsSaving = true;

            if (Component != null)
            {
                ComponentUpdateRequestModel componentUpdate = new();

                if (!string.IsNullOrWhiteSpace(EditComponentName) && EditComponentName != Component.Name)
                {
                    componentUpdate.Name = EditComponentName;

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Component Name: {Component.Name} -> {EditComponentName}");
                }

                (ComponentModel? component, ResponseModel? apiResponse) = await APIService.UpdateConfigurationEntity<ComponentModel, ComponentUpdateRequestModel>(
                    Entity,
                    Id,
                    componentUpdate);

                if (component != null)
                {
                    SaveSuccess = true;
                    Component = component;
                    EditComponentName = Component.Name;
                }

                else
                {
                    SaveSuccess = false;
                    ErrorFor = "Entity";
                    ErrorMessage = $"API returned {apiResponse?.StatusCode} ({apiResponse?.Message})";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        ErrorMessage);
                }

                await InvokeAsync(StateHasChanged);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Save Success: {SaveSuccess}");

                await Task.Delay(2000).ContinueWith(_ =>
                {
                    SaveSuccess = false;
                    ErrorFor = string.Empty;
                    ErrorMessage = string.Empty;

                    InvokeAsync(StateHasChanged);
                });
            }

            IsSaving = false;
        }

        /// <summary>
        /// Performs the connection update steps.
        /// </summary>
        private async Task SaveConnection()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                "Save Clicked");

            IsSaving = true;

            if (Connection != null)
            {
                ConnectionUpdateRequestModel connectionUpdate = new();

                if (!string.IsNullOrWhiteSpace(EditConnectionIP) && EditConnectionIP != Connection.IPAddress)
                {
                    connectionUpdate.IPAddress = EditConnectionIP;

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Connection Ip Address: {Connection.IPAddress} -> {EditConnectionIP}");
                }

                if (EditConnectionPort > 0 && EditConnectionPort != Connection.Port)
                {
                    connectionUpdate.Port = EditConnectionPort;

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Connection Port: {Connection.Port} -> {EditConnectionPort}");
                }

                (ConnectionModel? connection, ResponseModel? apiResponse) = await APIService.UpdateConfigurationEntity<ConnectionModel, ConnectionUpdateRequestModel>(
                    Entity,
                    Id,
                    connectionUpdate);

                if (connection != null)
                {
                    SaveSuccess = true;
                    Connection = connection;
                    EditConnectionIP = Connection.IPAddress;
                    EditConnectionPort = Connection.Port;
                }

                else
                {
                    SaveSuccess = false;
                    ErrorFor = "Entity";
                    ErrorMessage = $"API returned {apiResponse?.StatusCode} ({apiResponse?.Message})";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        ErrorMessage);
                }

                await InvokeAsync(StateHasChanged);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Save Success: {SaveSuccess}");

                await Task.Delay(2000).ContinueWith(_ =>
                {
                    SaveSuccess = false;
                    ErrorFor = string.Empty;
                    ErrorMessage = string.Empty;

                    InvokeAsync(StateHasChanged);
                });
            }

            IsSaving = false;
        }

        /// <summary>
        /// Performs the downtime update steps.
        /// </summary>
        private async Task SaveDowntime()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                "Save Clicked");

            IsSaving = true;

            if (Downtime != null)
            {
                DowntimeUpdateRequestModel downtimeUpdate = new();

                if (!string.IsNullOrWhiteSpace(EditDowntimeTime) && EditDowntimeTime != Downtime.Time)
                {
                    downtimeUpdate.Time = EditDowntimeTime;

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Downtime Time: {Downtime.Time} -> {EditDowntimeTime}");
                }

                if (EditDowntimeDuration > 0 && EditDowntimeDuration != Downtime.Duration)
                {
                    downtimeUpdate.Duration = EditDowntimeDuration;

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Downtime Duration: {Downtime.Duration} -> {EditDowntimeDuration}");
                }

                (DowntimeModel? downtime, ResponseModel? apiResponse) = await APIService.UpdateConfigurationEntity<DowntimeModel, DowntimeUpdateRequestModel>(
                    Entity,
                    Id,
                    downtimeUpdate);

                if (downtime != null)
                {
                    SaveSuccess = true;
                    Downtime = downtime;
                    EditDowntimeTime = Downtime.Time;
                    EditDowntimeDuration = Downtime.Duration;
                }

                else
                {
                    SaveSuccess = false;
                    ErrorFor = "Entity";
                    ErrorMessage = $"API returned {apiResponse?.StatusCode} ({apiResponse?.Message})";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        ErrorMessage);
                }

                await InvokeAsync(StateHasChanged);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Save Success: {SaveSuccess}");

                await Task.Delay(2000).ContinueWith(_ =>
                {
                    SaveSuccess = false;
                    ErrorFor = string.Empty;
                    ErrorMessage = string.Empty;

                    InvokeAsync(StateHasChanged);
                });
            }

            IsSaving = false;
        }

        /// <summary>
        /// Performs the game update steps.
        /// </summary>
        private async Task SaveGame()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                "Save Clicked");

            IsSaving = true;

            if (Game != null)
            {
                GameUpdateRequestModel gameUpdate = new();

                if (!string.IsNullOrWhiteSpace(EditGameName) && EditGameName != Game.Name)
                {
                    gameUpdate.Name = EditGameName;

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Game Name: {Game.Name} -> {EditGameName}");
                }

                if (!string.IsNullOrWhiteSpace(EditGameVersion) && EditGameVersion != Game.Version)
                {
                    gameUpdate.Version = EditGameVersion;

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Game Version: {Game.Version} -> {EditGameVersion}");
                }

                (GameModel? game, ResponseModel? apiResponse) = await APIService.UpdateConfigurationEntity<GameModel, GameUpdateRequestModel>(
                    Entity,
                    Id,
                    gameUpdate);

                if (game != null)
                {
                    SaveSuccess = true;
                    Game = game;
                    EditGameName = Game.Name;
                    EditGameVersion = Game.Version;
                }

                else
                {
                    SaveSuccess = false;
                    ErrorFor = "Entity";
                    ErrorMessage = $"API returned {apiResponse?.StatusCode} ({apiResponse?.Message})";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        ErrorMessage);
                }

                await InvokeAsync(StateHasChanged);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Save Success: {SaveSuccess}");

                await Task.Delay(2000).ContinueWith(_ =>
                {
                    SaveSuccess = false;
                    ErrorFor = string.Empty;
                    ErrorMessage = string.Empty;

                    InvokeAsync(StateHasChanged);
                });
            }

            IsSaving = false;
        }

        /// <summary>
        /// Performs the machine update steps.
        /// </summary>
        private async Task SaveMachine()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                "Save Clicked");

            IsSaving = true;

            if (Machine != null)
            {
                MachineUpdateRequestModel machineUpdate = new();

                if (!string.IsNullOrWhiteSpace(EditHostName) && EditHostName != Machine.HostName)
                {
                    machineUpdate.HostName = EditHostName;

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Machine Host Name: {Machine.HostName} -> {EditHostName}");
                }

                (MachineModel? machine, ResponseModel? apiResponse) = await APIService.UpdateConfigurationEntity<MachineModel, MachineUpdateRequestModel>(
                    Entity,
                    Id,
                    machineUpdate);

                if (machine != null)
                {
                    SaveSuccess = true;
                    Machine = machine;
                    EditHostName = Machine.HostName;
                }

                else
                {
                    SaveSuccess = false;
                    ErrorFor = "Entity";
                    ErrorMessage = $"API returned {apiResponse?.StatusCode} ({apiResponse?.Message})";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        ErrorMessage);
                }

                await InvokeAsync(StateHasChanged);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Save Success: {SaveSuccess}");

                await Task.Delay(2000).ContinueWith(_ =>
                {
                    SaveSuccess = false;
                    ErrorFor = string.Empty;
                    ErrorMessage = string.Empty;

                    InvokeAsync(StateHasChanged);
                });
            }

            IsSaving = false;
        }

        /// <summary>
        /// Starts adding the new setting.
        /// </summary>
        private void StartAddSetting()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                "Add Setting Clicked");

            NewSetting = new()
            {
                Name = string.Empty,
                Type = string.Empty,
                Required = false
            };
        }

        /// <summary>
        /// Removes the new setting.
        /// </summary>
        private void CancelAddSetting()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                "Cancel Clicked");

            NewSetting = null;
        }

        /// <summary>
        /// Performs the setting creation steps.
        /// </summary>
        private async Task ConfirmAddSetting()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                "Save Clicked");

            IsAddingSetting = true;
            ErrorFor = string.Empty;
            ErrorMessage = string.Empty;

            if (NewSetting != null && Application != null)
            {
                if (string.IsNullOrWhiteSpace(NewSetting.Name) || string.IsNullOrWhiteSpace(NewSetting.Type))
                {
                    ErrorFor = "AddSetting";
                    ErrorMessage = "Name and type are required.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        ErrorMessage);
                    IsAddingSetting = false;
                    return;
                }

                ApplicationSettingModel? existingSetting = Application.Settings.Find(s => s.Name == NewSetting.Name && !s.IsDeleted);

                if (existingSetting == null)
                {
                    (ApplicationSettingModel? applicationSetting, ResponseModel? apiResponse) = await APIService.CreateConfigurationEntity<ApplicationSettingModel, ApplicationSettingRequestModel>(
                        "applicationSetting",
                    NewSetting.Name,
                    NewSetting,
                    Id);

                    if (applicationSetting != null && Application != null)
                    {
                        AddSettingSuccess = true;
                        Application.Settings.Add(applicationSetting);
                    }

                    else
                    {
                        AddSettingSuccess = false;
                        ErrorMessage = $"API returned {apiResponse?.StatusCode} ({apiResponse?.Message})";
                        _Logger.LogMessage(
                            StandardValues.LoggerValues.Warning,
                            ErrorMessage);
                    }
                }

                else
                {
                    ErrorFor = "AddSetting";
                    ErrorMessage = "A setting with that name already exists.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        ErrorMessage);
                    IsAddingSetting = false;
                    return;
                }

                await InvokeAsync(StateHasChanged);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Save Success: {AddSettingSuccess}");

                if (AddSettingSuccess)
                {
                    await Task.Delay(2000).ContinueWith(_ =>
                    {
                        AddSettingSuccess = false;
                        ErrorFor = string.Empty;
                        ErrorMessage = string.Empty;
                        NewSetting = null;

                        InvokeAsync(StateHasChanged);
                    });
                }
            }

            IsAddingSetting = false;
        }

        /// <summary>
        /// Performs the setting update steps.
        /// </summary>
        private async Task SaveSetting(ApplicationSettingModel setting)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                "Save Clicked");

            IsSavingSetting = true;
            EditSettingId = setting.Id;
            ErrorFor = string.Empty;
            ErrorMessage = string.Empty;

            if (Application != null)
            {
                ApplicationSettingUpdateRequestModel settingUpdate = new();
                ApplicationSettingModel unchangedSetting = UnchangedSettings.First(us => us.Id == setting.Id);

                if (setting.Name != unchangedSetting.Name)
                {
                    settingUpdate.Name = setting.Name;

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Setting Name: {unchangedSetting.Name} -> {setting.Name}");
                }

                if (setting.Type != unchangedSetting.Type)
                {
                    settingUpdate.Type = setting.Type;

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Setting Type: {unchangedSetting.Type} -> {setting.Type}");
                }

                if (setting.Required != unchangedSetting.Required)
                {
                    settingUpdate.Required = setting.Required;

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Setting Required: {unchangedSetting.Required} -> {setting.Required}");
                }

                if (string.IsNullOrWhiteSpace(settingUpdate.Name) && string.IsNullOrWhiteSpace(settingUpdate.Type) && !settingUpdate.Required.HasValue)
                {
                    ErrorFor = $"SaveSetting {setting.Id}";
                    ErrorMessage = "Name, type or required are required.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        ErrorMessage);
                    IsSavingSetting = false;
                    return;
                }

                (ApplicationSettingModel? applicationSetting, ResponseModel? apiResponse) = await APIService.UpdateConfigurationEntity<ApplicationSettingModel, ApplicationSettingUpdateRequestModel>(
                    "applicationSetting",
                    setting.Id,
                    settingUpdate);

                if (applicationSetting != null && Application != null)
                {
                    int index = Application.Settings.FindIndex(s => s.Id == applicationSetting.Id);
                    Application.Settings[index] = applicationSetting;
                    unchangedSetting = applicationSetting;
                    SaveSettingSuccess = true;
                }

                else
                {
                    SaveSettingSuccess = false;
                    ErrorFor = $"SaveSetting {setting.Id}";
                    ErrorMessage = $"API returned {apiResponse?.StatusCode} ({apiResponse?.Message})";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        ErrorMessage);
                }

                await InvokeAsync(StateHasChanged);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Save Success: {SaveSettingSuccess}");

                if (SaveSettingSuccess)
                {
                    await Task.Delay(2000).ContinueWith(_ =>
                    {
                        SaveSettingSuccess = false;
                        ErrorFor = string.Empty;
                        ErrorMessage = string.Empty;

                        InvokeAsync(StateHasChanged);
                    });
                }
            }

            IsSavingSetting = false;
        }
    }
}
