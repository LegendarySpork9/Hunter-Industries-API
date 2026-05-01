// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Models;
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

        private string DisplayName = string.Empty;
        private bool LoadingData = true;
        private bool HasData = false;






        [Inject] private ExampleAPIService ExampleAPIService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        private string _displayName = string.Empty;
        private int? _saveSuccessId;
        private bool _showDeleteConfirm;
        private int _deleteTargetId;
        private bool _isAdding;

        // Entity lists
        private List<ConfigurationApplicationRecord> _applications = new();
        private List<ConfigurationAuthorisationRecord> _authorisations = new();
        private List<ConfigurationComponentRecord> _components = new();
        private List<ConfigurationConnectionRecord> _connections = new();
        private List<ConfigurationDowntimeRecord> _downtimes = new();
        private List<ConfigurationGameRecord> _games = new();
        private List<ConfigurationMachineRecord> _machines = new();

        // New record fields
        private string _newAppName = string.Empty;
        private string _newAppPhrase = string.Empty;
        private string _newAuthPhrase = string.Empty;
        private string _newComponentName = string.Empty;
        private string _newConnectionIP = string.Empty;
        private int _newConnectionPort;
        private string _newDowntimeTime = string.Empty;
        private int _newDowntimeDuration;
        private string _newGameName = string.Empty;
        private string _newGameVersion = string.Empty;
        private string _newMachineHostName = string.Empty;

        // Application setting add fields
        private int? _isAddingSettingForAppId;
        private string _newSettingName = string.Empty;
        private string _newSettingType = string.Empty;
        private bool _newSettingRequired;

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
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Opened User Page");

            DisplayName = DisplayNames.GetValueOrDefault(Entity, Entity);

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Entity: {Entity}");
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Display Name: {DisplayName}");

            await LoadData();

            if (Application != null || Authorisation != null || Component != null || Connection != null
                || Component != null || Downtime != null || Game != null || Machine != null)
            {
                HasData = true;
            }


            LoadingData = false;
        }

        /// <summary>
        /// Loads and transforms the configuration data.
        /// </summary>
        private async Task LoadData()
        {
            switch (Entity)
            {
                case "application":
                    Application = await APIService.GetConfigurationEntity<ApplicationModel?>(Entity,
                        Id);
                    break;
                case "authorisation":
                    Authorisation = await APIService.GetConfigurationEntity<AuthorisationModel?>(Entity,
                        Id);
                    break;
                case "component":
                    Component = await APIService.GetConfigurationEntity<ComponentModel?>(Entity,
                        Id);
                    break;
                case "connection":
                    Connection = await APIService.GetConfigurationEntity<ConnectionModel?>(Entity,
                        Id);
                    break;
                case "downtime":
                    Downtime = await APIService.GetConfigurationEntity<DowntimeModel?>(Entity,
                        Id);
                    break;
                case "game":
                    Game = await APIService.GetConfigurationEntity<GameModel?>(Entity,
                        Id);
                    break;
                case "machine":
                    Machine = await APIService.GetConfigurationEntity<MachineModel?>(Entity,
                        Id);
                    break;
            }
        }

        private void ShowSaveSuccess(int id)
        {
            _saveSuccessId = id;

            Task.Delay(2000).ContinueWith(_ =>
            {
                _saveSuccessId = null;
                InvokeAsync(StateHasChanged);
            });
        }

        // Add new record

        private void StartAdd()
        {
            _isAdding = true;
        }

        private void CancelAdd()
        {
            _isAdding = false;
            ResetNewFields();
        }

        private void ResetNewFields()
        {
            _newAppName = string.Empty;
            _newAppPhrase = string.Empty;
            _newAuthPhrase = string.Empty;
            _newComponentName = string.Empty;
            _newConnectionIP = string.Empty;
            _newConnectionPort = 0;
            _newDowntimeTime = string.Empty;
            _newDowntimeDuration = 0;
            _newGameName = string.Empty;
            _newGameVersion = string.Empty;
            _newMachineHostName = string.Empty;
        }

        private void ConfirmAdd()
        {
            switch (Entity)
            {
                case "application":
                    ExampleAPIService.CreateConfigurationApplication(_newAppName, _newAppPhrase);
                    break;
                case "authorisation":
                    ExampleAPIService.CreateConfigurationAuthorisation(_newAuthPhrase);
                    break;
                case "component":
                    ExampleAPIService.CreateConfigurationComponent(_newComponentName);
                    break;
                case "connection":
                    ExampleAPIService.CreateConfigurationConnection(_newConnectionIP, _newConnectionPort);
                    break;
                case "downtime":
                    ExampleAPIService.CreateConfigurationDowntime(_newDowntimeTime, _newDowntimeDuration);
                    break;
                case "game":
                    ExampleAPIService.CreateConfigurationGame(_newGameName, _newGameVersion);
                    break;
                case "machine":
                    ExampleAPIService.CreateConfigurationMachine(_newMachineHostName);
                    break;
            }

            _isAdding = false;
            ResetNewFields();
            LoadData();
        }

        // Delete

        private void ConfirmDelete(int id)
        {
            _deleteTargetId = id;
            _showDeleteConfirm = true;
        }

        private void CancelDelete()
        {
            _showDeleteConfirm = false;
        }

        private void ExecuteDelete()
        {
            switch (Entity)
            {
                case "application":
                    ExampleAPIService.DeleteConfigurationApplication(_deleteTargetId);
                    break;
                case "authorisation":
                    ExampleAPIService.DeleteConfigurationAuthorisation(_deleteTargetId);
                    break;
                case "component":
                    ExampleAPIService.DeleteConfigurationComponent(_deleteTargetId);
                    break;
                case "connection":
                    ExampleAPIService.DeleteConfigurationConnection(_deleteTargetId);
                    break;
                case "downtime":
                    ExampleAPIService.DeleteConfigurationDowntime(_deleteTargetId);
                    break;
                case "game":
                    ExampleAPIService.DeleteConfigurationGame(_deleteTargetId);
                    break;
                case "machine":
                    ExampleAPIService.DeleteConfigurationMachine(_deleteTargetId);
                    break;
            }

            _showDeleteConfirm = false;
            LoadData();
        }

        private void NavigateToLogs(int applicationId) => Navigation.NavigateTo($"/logs?application={applicationId}");

        // Save methods per entity type

        private void SaveApplication(ConfigurationApplicationRecord app)
        {
            ExampleAPIService.UpdateConfigurationApplication(app.Id, app.Name, app.Phrase);
            ShowSaveSuccess(app.Id);
        }

        private void SaveAuthorisation(ConfigurationAuthorisationRecord auth)
        {
            ExampleAPIService.UpdateConfigurationAuthorisation(auth.Id, auth.Phrase);
            ShowSaveSuccess(auth.Id);
        }

        private void SaveComponent(ConfigurationComponentRecord comp)
        {
            ExampleAPIService.UpdateConfigurationComponent(comp.Id, comp.Name);
            ShowSaveSuccess(comp.Id);
        }

        private void SaveConnection(ConfigurationConnectionRecord conn)
        {
            ExampleAPIService.UpdateConfigurationConnection(conn.Id, conn.IPAddress, conn.Port);
            ShowSaveSuccess(conn.Id);
        }

        private void SaveDowntime(ConfigurationDowntimeRecord dt)
        {
            ExampleAPIService.UpdateConfigurationDowntime(dt.Id, dt.Time, dt.Duration);
            ShowSaveSuccess(dt.Id);
        }

        private void SaveGame(ConfigurationGameRecord game)
        {
            ExampleAPIService.UpdateConfigurationGame(game.Id, game.Name, game.Version);
            ShowSaveSuccess(game.Id);
        }

        private void SaveMachine(ConfigurationMachineRecord machine)
        {
            ExampleAPIService.UpdateConfigurationMachine(machine.Id, machine.HostName);
            ShowSaveSuccess(machine.Id);
        }

        // Application settings

        private void StartAddSetting(int applicationId)
        {
            _isAddingSettingForAppId = applicationId;
            _newSettingName = string.Empty;
            _newSettingType = string.Empty;
            _newSettingRequired = false;
        }

        private void CancelAddSetting()
        {
            _isAddingSettingForAppId = null;
        }

        private void ConfirmAddSetting(int applicationId)
        {
            ExampleAPIService.CreateConfigurationApplicationSetting(applicationId, _newSettingName, _newSettingType, _newSettingRequired);
            _isAddingSettingForAppId = null;
            LoadData();
        }

        private void SaveSetting(int applicationId, ConfigurationApplicationSettingRecord setting)
        {
            ExampleAPIService.UpdateConfigurationApplicationSetting(applicationId, setting.Id, setting.Name, setting.Type, setting.Required);
            ShowSaveSuccess(setting.Id);
        }

        private void DeleteSetting(int applicationId, int settingId)
        {
            ExampleAPIService.DeleteConfigurationApplicationSetting(applicationId, settingId);
            LoadData();
        }
    }
}
