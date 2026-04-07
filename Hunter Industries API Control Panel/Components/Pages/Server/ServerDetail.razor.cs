using Microsoft.AspNetCore.Components;
using HunterIndustriesAPIControlPanel.Models;
using HunterIndustriesAPIControlPanel.Services;

namespace HunterIndustriesAPIControlPanel.Components.Pages.Server
{
    public partial class ServerDetail
    {
        [Parameter] public int Id { get; set; }
        [Inject] private APIService APIService { get; set; } = default!;

        private ServerInformationRecord? _server;
        private List<ServerAlertRecord> _alerts = new();
        private List<ServerEventRecord> _events = new();
        private List<ChartDataItem> _alertsByComponent = new();
        private List<ChartDataItem> _alertsByStatus = new();
        private string[] _alertStatusColours = Array.Empty<string>();
        private string[] _alertComponentColours = Array.Empty<string>();
        private List<ServerEventRecord> _lastEventPerComponent = new();

        private static readonly string[] DefaultPalette = new[]
        {
            "#4472C4", "#ED7D31", "#A5A5A5", "#FFC000", "#5B9BD5",
            "#70AD47", "#264478", "#9B57A0", "#636363", "#EB7E30"
        };

        private string _editName = string.Empty;
        private string _editHostName = string.Empty;
        private string _editGame = string.Empty;
        private string _editGameVersion = string.Empty;
        private string _editIPAddress = string.Empty;
        private int _editPort;
        private string _editDowntime = string.Empty;
        private int _editEventInterval;
        private bool _editIsActive;
        private bool _saveSuccess;

        protected override void OnInitialized()
        {
            LoadData();
        }

        private void LoadData()
        {
            _server = APIService.GetServer(Id);

            if (_server != null)
            {
                _editName = _server.Name;
                _editHostName = _server.HostName;
                _editGame = _server.Game;
                _editGameVersion = _server.GameVersion;
                _editIPAddress = _server.Connection.IPAddress;
                _editPort = _server.Connection.Port;
                _editDowntime = _server.Downtime?.Time ?? string.Empty;
                _editEventInterval = _server.EventInterval;
                _editIsActive = _server.IsActive;

                _alerts = APIService.GetServerAlerts(hostName: _server.HostName);
                _events = APIService.GetServerEvents(hostName: _server.HostName);

                _alertsByComponent = _alerts
                    .GroupBy(a => a.Component)
                    .Select(g => new ChartDataItem { Label = g.Key, Value = g.Count() })
                    .ToList();

                _alertComponentColours = _alertsByComponent
                    .Select((_, i) => DefaultPalette[i % DefaultPalette.Length])
                    .ToArray();

                _alertsByStatus = _alerts
                    .GroupBy(a => a.AlertStatus)
                    .Select(g => new ChartDataItem { Label = g.Key, Value = g.Count() })
                    .ToList();

                _alertStatusColours = _alertsByStatus
                    .Select(s => s.Label switch
                    {
                        "Resolved" => "#28a745",
                        "Reported" => "#dc3545",
                        "Investigating" => "#ffc107",
                        _ => "#6c757d"
                    })
                    .ToArray();

                _lastEventPerComponent = _events
                    .GroupBy(e => e.Component)
                    .Select(g => g.OrderByDescending(e => e.DateOccured).First())
                    .ToList();
            }
        }

        private void SaveServer()
        {
            if (_server == null) return;

            APIService.UpdateServer(_server.Id, _editName, _editHostName, _editGame, _editGameVersion,
                _editIPAddress, _editPort, string.IsNullOrEmpty(_editDowntime) ? null : _editDowntime, _editEventInterval, _editIsActive);

            _server = APIService.GetServer(Id);
            _saveSuccess = true;

            Task.Delay(2000).ContinueWith(_ =>
            {
                _saveSuccess = false;
                InvokeAsync(StateHasChanged);
            });
        }
    }
}
