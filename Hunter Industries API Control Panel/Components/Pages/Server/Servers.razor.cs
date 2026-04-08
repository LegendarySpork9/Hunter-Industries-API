using Microsoft.AspNetCore.Components;
using HunterIndustriesAPIControlPanel.Models;
using HunterIndustriesAPIControlPanel.Services;

namespace HunterIndustriesAPIControlPanel.Components.Pages.Server
{
    public partial class Servers
    {
        [Inject] private ExampleAPIService APIService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        private List<ServerInformationRecord> _servers = new();
        private bool _showModal;
        private bool _isEditing;
        private int _editingServerId;
        private string _modalName = string.Empty;
        private string _modalHostName = string.Empty;
        private string _modalGame = string.Empty;
        private string _modalGameVersion = string.Empty;
        private string _modalIPAddress = string.Empty;
        private int _modalPort;
        private string _modalDowntime = string.Empty;
        private int _modalEventInterval;
        private bool _modalIsActive = true;

        protected override void OnInitialized()
        {
            _servers = APIService.GetServers();
        }

        private void ShowCreateModal()
        {
            _isEditing = false;
            _modalName = string.Empty;
            _modalHostName = string.Empty;
            _modalGame = string.Empty;
            _modalGameVersion = string.Empty;
            _modalIPAddress = string.Empty;
            _modalPort = 25565;
            _modalDowntime = string.Empty;
            _modalEventInterval = 0;
            _modalIsActive = true;
            _showModal = true;
        }

        private void ShowEditModal(ServerInformationRecord server)
        {
            _isEditing = true;
            _editingServerId = server.Id;
            _modalName = server.Name;
            _modalHostName = server.HostName;
            _modalGame = server.Game;
            _modalGameVersion = server.GameVersion;
            _modalIPAddress = server.Connection.IPAddress;
            _modalPort = server.Connection.Port;
            _modalDowntime = server.Downtime?.Time ?? string.Empty;
            _modalEventInterval = server.EventInterval;
            _modalIsActive = server.IsActive;
            _showModal = true;
        }

        private void CloseModal() => _showModal = false;

        private void SaveServer()
        {
            if (_isEditing)
            {
                APIService.UpdateServer(_editingServerId, _modalName, _modalHostName, _modalGame, _modalGameVersion,
                    _modalIPAddress, _modalPort, string.IsNullOrEmpty(_modalDowntime) ? null : _modalDowntime, _modalEventInterval, _modalIsActive);
            }
            else
            {
                APIService.CreateServer(_modalName, _modalHostName, _modalGame, _modalGameVersion,
                    _modalIPAddress, _modalPort, string.IsNullOrEmpty(_modalDowntime) ? null : _modalDowntime, _modalEventInterval, _modalIsActive);
            }

            _servers = APIService.GetServers();
            _showModal = false;
        }
    }
}
