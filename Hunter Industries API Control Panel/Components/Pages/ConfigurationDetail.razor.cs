using Microsoft.AspNetCore.Components;
using Hunter_Industries_API_Control_Panel.Models;
using Hunter_Industries_API_Control_Panel.Services;

namespace Hunter_Industries_API_Control_Panel.Components.Pages
{
    public partial class ConfigurationDetail
    {
        [Parameter] public string Entity { get; set; } = string.Empty;
        [Inject] private APIService APIService { get; set; } = default!;
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

        protected override void OnInitialized()
        {
            _displayName = DisplayNames.GetValueOrDefault(Entity, Entity);
            LoadData();
        }

        private void LoadData()
        {
            switch (Entity)
            {
                case "application":
                    _applications = APIService.GetConfigurationApplications();
                    break;
                case "authorisation":
                    _authorisations = APIService.GetConfigurationAuthorisations();
                    break;
                case "component":
                    _components = APIService.GetConfigurationComponents();
                    break;
                case "connection":
                    _connections = APIService.GetConfigurationConnections();
                    break;
                case "downtime":
                    _downtimes = APIService.GetConfigurationDowntimes();
                    break;
                case "game":
                    _games = APIService.GetConfigurationGames();
                    break;
                case "machine":
                    _machines = APIService.GetConfigurationMachines();
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
                    APIService.CreateConfigurationApplication(_newAppName, _newAppPhrase);
                    break;
                case "authorisation":
                    APIService.CreateConfigurationAuthorisation(_newAuthPhrase);
                    break;
                case "component":
                    APIService.CreateConfigurationComponent(_newComponentName);
                    break;
                case "connection":
                    APIService.CreateConfigurationConnection(_newConnectionIP, _newConnectionPort);
                    break;
                case "downtime":
                    APIService.CreateConfigurationDowntime(_newDowntimeTime, _newDowntimeDuration);
                    break;
                case "game":
                    APIService.CreateConfigurationGame(_newGameName, _newGameVersion);
                    break;
                case "machine":
                    APIService.CreateConfigurationMachine(_newMachineHostName);
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
                    APIService.DeleteConfigurationApplication(_deleteTargetId);
                    break;
                case "authorisation":
                    APIService.DeleteConfigurationAuthorisation(_deleteTargetId);
                    break;
                case "component":
                    APIService.DeleteConfigurationComponent(_deleteTargetId);
                    break;
                case "connection":
                    APIService.DeleteConfigurationConnection(_deleteTargetId);
                    break;
                case "downtime":
                    APIService.DeleteConfigurationDowntime(_deleteTargetId);
                    break;
                case "game":
                    APIService.DeleteConfigurationGame(_deleteTargetId);
                    break;
                case "machine":
                    APIService.DeleteConfigurationMachine(_deleteTargetId);
                    break;
            }

            _showDeleteConfirm = false;
            LoadData();
        }

        private void NavigateToLogs(int applicationId) => Navigation.NavigateTo($"/logs?application={applicationId}");

        // Save methods per entity type

        private void SaveApplication(ConfigurationApplicationRecord app)
        {
            APIService.UpdateConfigurationApplication(app.Id, app.Name, app.Phrase);
            ShowSaveSuccess(app.Id);
        }

        private void SaveAuthorisation(ConfigurationAuthorisationRecord auth)
        {
            APIService.UpdateConfigurationAuthorisation(auth.Id, auth.Phrase);
            ShowSaveSuccess(auth.Id);
        }

        private void SaveComponent(ConfigurationComponentRecord comp)
        {
            APIService.UpdateConfigurationComponent(comp.Id, comp.Name);
            ShowSaveSuccess(comp.Id);
        }

        private void SaveConnection(ConfigurationConnectionRecord conn)
        {
            APIService.UpdateConfigurationConnection(conn.Id, conn.IPAddress, conn.Port);
            ShowSaveSuccess(conn.Id);
        }

        private void SaveDowntime(ConfigurationDowntimeRecord dt)
        {
            APIService.UpdateConfigurationDowntime(dt.Id, dt.Time, dt.Duration);
            ShowSaveSuccess(dt.Id);
        }

        private void SaveGame(ConfigurationGameRecord game)
        {
            APIService.UpdateConfigurationGame(game.Id, game.Name, game.Version);
            ShowSaveSuccess(game.Id);
        }

        private void SaveMachine(ConfigurationMachineRecord machine)
        {
            APIService.UpdateConfigurationMachine(machine.Id, machine.HostName);
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
            APIService.CreateConfigurationApplicationSetting(applicationId, _newSettingName, _newSettingType, _newSettingRequired);
            _isAddingSettingForAppId = null;
            LoadData();
        }

        private void SaveSetting(int applicationId, ConfigurationApplicationSettingRecord setting)
        {
            APIService.UpdateConfigurationApplicationSetting(applicationId, setting.Id, setting.Name, setting.Type, setting.Required);
            ShowSaveSuccess(setting.Id);
        }

        private void DeleteSetting(int applicationId, int settingId)
        {
            APIService.DeleteConfigurationApplicationSetting(applicationId, settingId);
            LoadData();
        }
    }
}
