using Hunter_Industries_API_Control_Panel.Models;

namespace Hunter_Industries_API_Control_Panel.Services
{
    public class APIService
    {
        private readonly LoggerService _logger;

        private static readonly List<string> AvailableScopes = new()
        {
            "User", "Assistant API", "Book Reader API", "Control Panel API", "Server Status API"
        };

        private static readonly List<UserRecord> Users = new()
        {
            new UserRecord { Id = 1, Username = "admin", Password = "hashed_admin_pass", Scopes = new List<string> { "User", "Assistant API", "Control Panel API", "Server Status API" }, IsDeleted = false },
            new UserRecord { Id = 2, Username = "hunter.bot", Password = "hashed_hunter_pass", Scopes = new List<string> { "Assistant API", "Server Status API" }, IsDeleted = false },
            new UserRecord { Id = 3, Username = "server.monitor", Password = "hashed_monitor_pass", Scopes = new List<string> { "Server Status API" }, IsDeleted = false },
            new UserRecord { Id = 4, Username = "api.user", Password = "hashed_api_pass", Scopes = new List<string> { "User", "Book Reader API" }, IsDeleted = false },
            new UserRecord { Id = 5, Username = "test.user", Password = "hashed_test_pass", Scopes = new List<string> { "User" }, IsDeleted = true }
        };

        private static readonly Dictionary<string, List<UserSettingRecord>> UserSettings = new()
        {
            ["admin"] = new List<UserSettingRecord>
            {
                new UserSettingRecord
                {
                    Application = "Hunter Assistant",
                    Settings = new List<SettingRecord>
                    {
                        new SettingRecord { Id = 1, Name = "DarkMode", Value = "true" },
                        new SettingRecord { Id = 2, Name = "Language", Value = "English" },
                        new SettingRecord { Id = 3, Name = "NotificationsEnabled", Value = "true" }
                    }
                },
                new UserSettingRecord
                {
                    Application = "API Dashboard",
                    Settings = new List<SettingRecord>
                    {
                        new SettingRecord { Id = 4, Name = "PageSize", Value = "25" },
                        new SettingRecord { Id = 5, Name = "DefaultView", Value = "grid" },
                        new SettingRecord { Id = 6, Name = "ShowCharts", Value = "true" }
                    }
                }
            },
            ["hunter.bot"] = new List<UserSettingRecord>
            {
                new UserSettingRecord
                {
                    Application = "Hunter Assistant",
                    Settings = new List<SettingRecord>
                    {
                        new SettingRecord { Id = 7, Name = "DarkMode", Value = "false" },
                        new SettingRecord { Id = 8, Name = "Language", Value = "English" },
                        new SettingRecord { Id = 9, Name = "NotificationsEnabled", Value = "false" }
                    }
                }
            },
            ["server.monitor"] = new List<UserSettingRecord>
            {
                new UserSettingRecord
                {
                    Application = "Server Monitor",
                    Settings = new List<SettingRecord>
                    {
                        new SettingRecord { Id = 10, Name = "RefreshInterval", Value = "30" },
                        new SettingRecord { Id = 11, Name = "AlertThreshold", Value = "5" },
                        new SettingRecord { Id = 12, Name = "AutoRestart", Value = "false" },
                        new SettingRecord { Id = 13, Name = "DiscordName", Value = "ServerBot#1234" }
                    }
                }
            },
            ["api.user"] = new List<UserSettingRecord>
            {
                new UserSettingRecord
                {
                    Application = "API Dashboard",
                    Settings = new List<SettingRecord>
                    {
                        new SettingRecord { Id = 14, Name = "PageSize", Value = "10" },
                        new SettingRecord { Id = 15, Name = "DefaultView", Value = "list" },
                        new SettingRecord { Id = 16, Name = "ShowCharts", Value = "false" }
                    }
                }
            }
        };

        private static readonly List<ServerInformationRecord> Servers = new()
        {
            new ServerInformationRecord { Id = 1, HostName = "MC-SURVIVAL-01", Game = "Minecraft", GameVersion = "1.20.4", Connection = new ConnectionRecord { IPAddress = "192.168.1.10", Port = 25565 }, Downtime = new DowntimeRecord { Time = "03:00:00" }, IsActive = true },
            new ServerInformationRecord { Id = 2, HostName = "MC-CREATIVE-01", Game = "Minecraft", GameVersion = "1.20.4", Connection = new ConnectionRecord { IPAddress = "192.168.1.11", Port = 25566 }, Downtime = new DowntimeRecord { Time = "04:00:00" }, IsActive = true },
            new ServerInformationRecord { Id = 3, HostName = "VH-EXPLORE-01", Game = "Valheim", GameVersion = "0.217.46", Connection = new ConnectionRecord { IPAddress = "192.168.1.20", Port = 2456 }, Downtime = null, IsActive = true },
            new ServerInformationRecord { Id = 4, HostName = "MC-MODDED-01", Game = "Minecraft", GameVersion = "1.19.2", Connection = new ConnectionRecord { IPAddress = "192.168.1.12", Port = 25567 }, Downtime = new DowntimeRecord { Time = "05:00:00" }, IsActive = false }
        };

        private static readonly List<ServerAlertRecord> ServerAlerts;
        private static readonly List<ServerEventRecord> ServerEvents;
        private static readonly List<AuditHistoryRecord> AuditHistory;
        private static readonly List<ErrorLogRecord> ErrorLogs;

        private static int _nextUserId = 6;
        private static int _nextServerId = 5;
        private static int _nextAlertId = 11;
        private static int _nextSettingId = 17;

        static APIService()
        {
            var now = DateTime.UtcNow;
            var random = new Random(42);

            ServerAlerts = new List<ServerAlertRecord>
            {
                new() { AlertId = 1, Reporter = "ServerBot#1234", Component = "PC Status", ComponentStatus = "Offline", AlertStatus = "Resolved", AlertDate = now.AddDays(-25), Server = new RelatedServerRecord { HostName = "MC-SURVIVAL-01", Game = "Minecraft", GameVersion = "1.20.4" } },
                new() { AlertId = 2, Reporter = "ServerBot#1234", Component = "Server Status", ComponentStatus = "Offline", AlertStatus = "Resolved", AlertDate = now.AddDays(-22), Server = new RelatedServerRecord { HostName = "MC-SURVIVAL-01", Game = "Minecraft", GameVersion = "1.20.4" } },
                new() { AlertId = 3, Reporter = "ServerBot#1234", Component = "Connection Status", ComponentStatus = "Unknown", AlertStatus = "Investigating", AlertDate = now.AddDays(-18), Server = new RelatedServerRecord { HostName = "VH-EXPLORE-01", Game = "Valheim", GameVersion = "0.217.46" } },
                new() { AlertId = 4, Reporter = "AdminUser", Component = "PC Status", ComponentStatus = "Offline", AlertStatus = "Resolved", AlertDate = now.AddDays(-15), Server = new RelatedServerRecord { HostName = "MC-CREATIVE-01", Game = "Minecraft", GameVersion = "1.20.4" } },
                new() { AlertId = 5, Reporter = "ServerBot#1234", Component = "Server Status", ComponentStatus = "Offline", AlertStatus = "Reported", AlertDate = now.AddDays(-10), Server = new RelatedServerRecord { HostName = "MC-MODDED-01", Game = "Minecraft", GameVersion = "1.19.2" } },
                new() { AlertId = 6, Reporter = "ServerBot#1234", Component = "Connection Status", ComponentStatus = "Offline", AlertStatus = "Resolved", AlertDate = now.AddDays(-8), Server = new RelatedServerRecord { HostName = "MC-MODDED-01", Game = "Minecraft", GameVersion = "1.19.2" } },
                new() { AlertId = 7, Reporter = "AdminUser", Component = "PC Status", ComponentStatus = "Online", AlertStatus = "Resolved", AlertDate = now.AddDays(-5), Server = new RelatedServerRecord { HostName = "MC-SURVIVAL-01", Game = "Minecraft", GameVersion = "1.20.4" } },
                new() { AlertId = 8, Reporter = "ServerBot#1234", Component = "Server Status", ComponentStatus = "Unknown", AlertStatus = "Investigating", AlertDate = now.AddDays(-3), Server = new RelatedServerRecord { HostName = "VH-EXPLORE-01", Game = "Valheim", GameVersion = "0.217.46" } },
                new() { AlertId = 9, Reporter = "ServerBot#1234", Component = "Connection Status", ComponentStatus = "Offline", AlertStatus = "Reported", AlertDate = now.AddDays(-1), Server = new RelatedServerRecord { HostName = "MC-CREATIVE-01", Game = "Minecraft", GameVersion = "1.20.4" } },
                new() { AlertId = 10, Reporter = "AdminUser", Component = "PC Status", ComponentStatus = "Offline", AlertStatus = "Reported", AlertDate = now.AddHours(-6), Server = new RelatedServerRecord { HostName = "MC-MODDED-01", Game = "Minecraft", GameVersion = "1.19.2" } }
            };

            ServerEvents = new List<ServerEventRecord>();
            var components = new[] { "PC Status", "Server Status", "Connection Status" };
            var statuses = new[] { "Online", "Offline", "Unknown" };
            var serverRefs = new[]
            {
                new RelatedServerRecord { HostName = "MC-SURVIVAL-01", Game = "Minecraft", GameVersion = "1.20.4" },
                new RelatedServerRecord { HostName = "MC-CREATIVE-01", Game = "Minecraft", GameVersion = "1.20.4" },
                new RelatedServerRecord { HostName = "VH-EXPLORE-01", Game = "Valheim", GameVersion = "0.217.46" },
                new RelatedServerRecord { HostName = "MC-MODDED-01", Game = "Minecraft", GameVersion = "1.19.2" }
            };

            for (int i = 0; i < 15; i++)
            {
                ServerEvents.Add(new ServerEventRecord
                {
                    Component = components[random.Next(components.Length)],
                    Status = statuses[random.Next(statuses.Length)],
                    DateOccured = now.AddDays(-random.Next(30)).AddHours(-random.Next(24)),
                    Server = serverRefs[random.Next(serverRefs.Length)]
                });
            }

            var endpoints = new[] { "/api/v1.0/auth/token", "/api/v1.0/user", "/api/v2.0/user", "/api/v2.0/usersettings", "/api/v1.1/serverstatus/serverinformation", "/api/v1.1/serverstatus/serveralert", "/api/v1.1/serverstatus/serverevent" };
            var methods = new[] { "GET", "GET", "GET", "GET", "POST", "POST", "PATCH", "DELETE" };
            var statusCodes = new[] { "200 OK", "200 OK", "200 OK", "200 OK", "200 OK", "201 Created", "400 Bad Request", "401 Unauthorized", "401 Unauthorized", "500 Internal Server Error" };
            var ips = new[] { "192.168.1.100", "10.0.0.50", "172.16.0.25", "192.168.1.200", "6.144.200.221", "20.68.171.243" };

            AuditHistory = new List<AuditHistoryRecord>();

            for (int i = 1; i <= 50; i++)
            {
                var endpoint = endpoints[random.Next(endpoints.Length)];
                var method = endpoint == "/api/v1.0/auth/token" ? "POST" : methods[random.Next(methods.Length)];
                var status = statusCodes[random.Next(statusCodes.Length)];
                var daysAgo = random.Next(60);

                var record = new AuditHistoryRecord
                {
                    Id = i,
                    IPAddress = ips[random.Next(ips.Length)],
                    Endpoint = endpoint,
                    Method = method,
                    Status = status,
                    OccuredAt = now.AddDays(-daysAgo).AddHours(-random.Next(24)).AddMinutes(-random.Next(60)),
                    Paramaters = random.Next(3) == 0 ? new[] { $"PageSize=10", $"PageNumber=1" } : null
                };

                if (endpoint == "/api/v1.0/auth/token" && i <= 10)
                {
                    var usernames = new[] { "admin", "hunter.bot", "server.monitor", "api.user", "unknown.user" };
                    record.LoginAttempt = new LoginAttemptRecord
                    {
                        Id = i,
                        Username = usernames[random.Next(usernames.Length)],
                        Phrase = "••••••••",
                        IsSuccessful = status == "200 OK"
                    };
                }

                if ((method == "PATCH" || method == "POST" || method == "DELETE") && i > 10 && i <= 18)
                {
                    var fields = new[] { "Password", "Username", "Scopes", "AlertStatus", "GameVersion", "Setting Value" };
                    record.Change = new List<ChangeRecord>
                    {
                        new ChangeRecord
                        {
                            Id = i,
                            Endpoint = endpoint,
                            Field = fields[random.Next(fields.Length)],
                            OldValue = "OldValue_" + random.Next(100),
                            NewValue = "NewValue_" + random.Next(100)
                        }
                    };
                }

                AuditHistory.Add(record);
            }

            AuditHistory = AuditHistory.OrderByDescending(a => a.OccuredAt).ToList();

            var errorSummaries = new[] {
                "An error occured when trying to run ServerEventService.GetServerEvents.",
                "An error occured when trying to run ServerEventService.LogServerEvent.",
                "An error occured when trying to run ServerInformationService.GetServer.",
                "An error occured when trying to run ServerInformationService.GetServers.",
                "An error occured when trying to run UserService.GetUser.",
                "An error occured when trying to run AuditHistoryService.GetAuditHistory."
            };

            ErrorLogs = new List<ErrorLogRecord>();

            for (int i = 1; i <= 15; i++)
            {
                var summary = errorSummaries[random.Next(errorSummaries.Length)];
                ErrorLogs.Add(new ErrorLogRecord
                {
                    Id = i,
                    DateOccured = now.AddDays(-random.Next(30)).AddHours(-random.Next(24)),
                    IPAddress = ips[random.Next(ips.Length)],
                    Summary = summary,
                    Message = $"{summary}\n\nSystem.Exception: {summary.Replace("An error occured when trying to run ", "")}\n   at Hunter_Industries_API.Services.{summary.Replace("An error occured when trying to run ", "").Replace(".", ".")}()\n   at Hunter_Industries_API.Controllers.ServerStatusController.Get()\n   at System.Web.Http.Controllers.ApiControllerActionInvoker.InvokeActionAsync()\n   at System.Threading.Tasks.TaskHelpersExtensions.CastToObject(Task`1 task)"
                });
            }

            ErrorLogs = ErrorLogs.OrderByDescending(e => e.DateOccured).ToList();
        }

        public APIService(LoggerService logger)
        {
            _logger = logger;
        }

        public (bool Success, string Token) Authenticate(string username, string password, string phrase)
        {
            _logger.LogInfo($"Authentication attempt for user: {username}");

            var user = Users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && !u.IsDeleted);

            if (user != null && phrase == "HunterIndustries")
            {
                return (true, $"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.dummy.token.{Guid.NewGuid():N}");
            }

            _logger.LogWarning($"Failed authentication attempt for user: {username}");
            return (false, string.Empty);
        }

        public DashboardSummary GetDashboardSummary()
        {
            var now = DateTime.UtcNow;
            var thisMonth = AuditHistory.Where(a => a.OccuredAt.Year == now.Year && a.OccuredAt.Month == now.Month).ToList();
            var lastMonth = AuditHistory.Where(a => a.OccuredAt.Year == now.AddMonths(-1).Year && a.OccuredAt.Month == now.AddMonths(-1).Month).ToList();

            var summary = new DashboardSummary
            {
                ApplicationCount = 3,
                ActiveUserCount = Users.Count(u => !u.IsDeleted),
                CallsThisMonth = thisMonth.Count,
                CallsLastMonth = lastMonth.Count,
                LoginAttemptsThisMonth = thisMonth.Count(a => a.LoginAttempt != null),
                LoginAttemptsLastMonth = lastMonth.Count(a => a.LoginAttempt != null),
                ChangesThisMonth = thisMonth.Count(a => a.Change != null && a.Change.Count > 0),
                ChangesLastMonth = lastMonth.Count(a => a.Change != null && a.Change.Count > 0),
                ErrorsThisMonth = ErrorLogs.Count(e => e.DateOccured.Year == now.Year && e.DateOccured.Month == now.Month),
                ErrorsLastMonth = ErrorLogs.Count(e => e.DateOccured.Year == now.AddMonths(-1).Year && e.DateOccured.Month == now.AddMonths(-1).Month)
            };

            var last30Days = Enumerable.Range(0, 30).Select(i => now.Date.AddDays(-i)).Reverse().ToList();
            summary.DailyTraffic = last30Days.Select(date => new TrafficDataPoint
            {
                Date = date,
                SuccessCount = AuditHistory.Count(a => a.OccuredAt.Date == date && (a.Status.StartsWith("200") || a.Status.StartsWith("201"))),
                FailureCount = AuditHistory.Count(a => a.OccuredAt.Date == date && !a.Status.StartsWith("200") && !a.Status.StartsWith("201"))
            }).ToList();

            summary.ErrorsByIPAndSummary = ErrorLogs
                .GroupBy(e => new { e.IPAddress, e.Summary })
                .Select(g => new ChartDataItem { Label = g.Key.IPAddress, Value = g.Count(), Category = g.Key.Summary.Length > 50 ? g.Key.Summary[..50] + "..." : g.Key.Summary })
                .ToList();

            summary.CallsByEndpoint = AuditHistory
                .GroupBy(a => a.Endpoint.Split('/').Last())
                .Select(g => new ChartDataItem { Label = g.Key, Value = g.Count() })
                .OrderByDescending(c => c.Value)
                .Take(5)
                .ToList();

            summary.CallsByMethod = AuditHistory
                .GroupBy(a => a.Method)
                .Select(g => new ChartDataItem { Label = g.Key, Value = g.Count() })
                .ToList();

            summary.CallsByStatusCode = AuditHistory
                .GroupBy(a => a.Status)
                .Select(g => new ChartDataItem { Label = g.Key, Value = g.Count() })
                .ToList();

            summary.ChangesByField = AuditHistory
                .Where(a => a.Change != null)
                .SelectMany(a => a.Change!)
                .GroupBy(c => c.Field)
                .Select(g => new ChartDataItem { Label = g.Key, Value = g.Count() })
                .ToList();

            var apps = new[] { "Server Status Site", "API Admin", "Control Panel" };
            var rand = new Random(42);
            summary.LoginAttemptsByUser = AuditHistory
                .Where(a => a.LoginAttempt != null)
                .Select(a => new ChartDataItem
                {
                    Label = a.LoginAttempt!.Username,
                    Value = 1,
                    Category = apps[rand.Next(apps.Length)]
                })
                .GroupBy(c => new { c.Label, c.Category })
                .Select(g => new ChartDataItem { Label = g.Key.Label, Value = g.Sum(x => x.Value), Category = g.Key.Category })
                .ToList();

            summary.ServerHealth = Servers.Where(s => s.IsActive).Select(s => new ServerHealthDataPoint
            {
                ServerName = s.HostName,
                UptimePercent = s.HostName switch { "MC-SURVIVAL-01" => 98.5, "MC-CREATIVE-01" => 95.2, "VH-EXPLORE-01" => 99.1, _ => 85.0 },
                AlertCount = ServerAlerts.Count(a => a.Server.HostName == s.HostName),
                EventCount = ServerEvents.Count(e => e.Server.HostName == s.HostName)
            }).ToList();

            return summary;
        }

        public List<UserRecord> GetUsers(string? username = null)
        {
            var query = Users.AsEnumerable();

            if (!string.IsNullOrEmpty(username))
            {
                query = query.Where(u => u.Username.Contains(username, StringComparison.OrdinalIgnoreCase));
            }

            return query.ToList();
        }

        public UserRecord? GetUser(int id) => Users.FirstOrDefault(u => u.Id == id);

        public List<string> GetAvailableScopes() => AvailableScopes;

        public bool CreateUser(string username, string password, List<string> scopes)
        {
            if (Users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            Users.Add(new UserRecord
            {
                Id = _nextUserId++,
                Username = username,
                Password = password,
                Scopes = scopes,
                IsDeleted = false
            });

            _logger.LogInfo($"User created: {username}");
            return true;
        }

        public bool UpdateUser(int id, string? username, string? password, List<string>? scopes)
        {
            var user = Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return false;

            if (!string.IsNullOrEmpty(username)) user.Username = username;
            if (!string.IsNullOrEmpty(password)) user.Password = password;
            if (scopes != null) user.Scopes = scopes;

            _logger.LogInfo($"User updated: {user.Username}");
            return true;
        }

        public bool DeleteUser(int id)
        {
            var user = Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return false;

            user.IsDeleted = true;
            _logger.LogInfo($"User deleted: {user.Username}");
            return true;
        }

        public List<UserSettingRecord> GetUserSettings(string username, string? application = null)
        {
            if (!UserSettings.TryGetValue(username, out var settings))
            {
                return new List<UserSettingRecord>();
            }

            if (!string.IsNullOrEmpty(application))
            {
                return settings.Where(s => s.Application.Equals(application, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return settings;
        }

        public List<UserSettingRecord> GetAllApplicationSettings()
        {
            return UserSettings.Values
                .SelectMany(s => s)
                .GroupBy(s => s.Application)
                .Select(g => new UserSettingRecord
                {
                    Application = g.Key,
                    Settings = g.SelectMany(a => a.Settings)
                        .GroupBy(s => s.Name)
                        .Select(sg => new SettingRecord { Id = 0, Name = sg.Key, Value = string.Empty })
                        .ToList()
                })
                .ToList();
        }

        public bool AddUserSetting(string username, string application, string settingName, string value)
        {
            if (!UserSettings.ContainsKey(username))
            {
                UserSettings[username] = new List<UserSettingRecord>();
            }

            var appSettings = UserSettings[username].FirstOrDefault(s => s.Application == application);
            if (appSettings == null)
            {
                appSettings = new UserSettingRecord { Application = application, Settings = new List<SettingRecord>() };
                UserSettings[username].Add(appSettings);
            }

            if (appSettings.Settings.Any(s => s.Name == settingName)) return false;

            appSettings.Settings.Add(new SettingRecord { Id = _nextSettingId++, Name = settingName, Value = value });
            _logger.LogInfo($"Setting added for {username}/{application}: {settingName} = {value}");
            return true;
        }

        public bool UpdateUserSetting(string username, string application, int settingId, string value)
        {
            if (!UserSettings.TryGetValue(username, out var settings)) return false;

            var appSettings = settings.FirstOrDefault(s => s.Application == application);
            var setting = appSettings?.Settings.FirstOrDefault(s => s.Id == settingId);
            if (setting == null) return false;

            setting.Value = value;
            _logger.LogInfo($"Setting updated for {username}/{application}: {setting.Name} = {value}");
            return true;
        }

        public List<ServerInformationRecord> GetServers(string? hostName = null, string? game = null)
        {
            var query = Servers.AsEnumerable();

            if (!string.IsNullOrEmpty(hostName))
                query = query.Where(s => s.HostName.Contains(hostName, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(game))
                query = query.Where(s => s.Game.Contains(game, StringComparison.OrdinalIgnoreCase));

            return query.ToList();
        }

        public ServerInformationRecord? GetServer(int id) => Servers.FirstOrDefault(s => s.Id == id);

        public bool CreateServer(string hostName, string game, string gameVersion, string ipAddress, int port, string? downtime, bool isActive)
        {
            Servers.Add(new ServerInformationRecord
            {
                Id = _nextServerId++,
                HostName = hostName,
                Game = game,
                GameVersion = gameVersion,
                Connection = new ConnectionRecord { IPAddress = ipAddress, Port = port },
                Downtime = downtime != null ? new DowntimeRecord { Time = downtime } : null,
                IsActive = isActive
            });

            _logger.LogInfo($"Server created: {hostName}");
            return true;
        }

        public bool UpdateServer(int id, string? hostName, string? game, string? gameVersion, string? ipAddress, int? port, string? downtime, bool? isActive)
        {
            var server = Servers.FirstOrDefault(s => s.Id == id);
            if (server == null) return false;

            if (!string.IsNullOrEmpty(hostName)) server.HostName = hostName;
            if (!string.IsNullOrEmpty(game)) server.Game = game;
            if (!string.IsNullOrEmpty(gameVersion)) server.GameVersion = gameVersion;
            if (!string.IsNullOrEmpty(ipAddress)) server.Connection.IPAddress = ipAddress;
            if (port.HasValue) server.Connection.Port = port.Value;
            if (downtime != null) server.Downtime = new DowntimeRecord { Time = downtime };
            if (isActive.HasValue) server.IsActive = isActive.Value;

            _logger.LogInfo($"Server updated: {server.HostName}");
            return true;
        }

        public List<ServerAlertRecord> GetServerAlerts(string? hostName = null, string? component = null, string? alertStatus = null)
        {
            var query = ServerAlerts.AsEnumerable();

            if (!string.IsNullOrEmpty(hostName))
                query = query.Where(a => a.Server.HostName.Equals(hostName, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(component))
                query = query.Where(a => a.Component.Equals(component, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(alertStatus))
                query = query.Where(a => a.AlertStatus.Equals(alertStatus, StringComparison.OrdinalIgnoreCase));

            return query.OrderByDescending(a => a.AlertDate).ToList();
        }

        public bool CreateServerAlert(string reporter, string component, string componentStatus, string alertStatus, string hostName)
        {
            var server = Servers.FirstOrDefault(s => s.HostName.Equals(hostName, StringComparison.OrdinalIgnoreCase));
            if (server == null) return false;

            ServerAlerts.Add(new ServerAlertRecord
            {
                AlertId = _nextAlertId++,
                Reporter = reporter,
                Component = component,
                ComponentStatus = componentStatus,
                AlertStatus = alertStatus,
                AlertDate = DateTime.UtcNow,
                Server = new RelatedServerRecord { HostName = server.HostName, Game = server.Game, GameVersion = server.GameVersion }
            });

            return true;
        }

        public bool UpdateServerAlert(int alertId, string? alertStatus, string? componentStatus)
        {
            var alert = ServerAlerts.FirstOrDefault(a => a.AlertId == alertId);
            if (alert == null) return false;

            if (!string.IsNullOrEmpty(alertStatus)) alert.AlertStatus = alertStatus;
            if (!string.IsNullOrEmpty(componentStatus)) alert.ComponentStatus = componentStatus;
            return true;
        }

        public bool DeleteServerAlert(int alertId)
        {
            var alert = ServerAlerts.FirstOrDefault(a => a.AlertId == alertId);
            if (alert == null) return false;

            ServerAlerts.Remove(alert);
            return true;
        }

        public List<ServerEventRecord> GetServerEvents(string? hostName = null, string? component = null)
        {
            var query = ServerEvents.AsEnumerable();

            if (!string.IsNullOrEmpty(hostName))
                query = query.Where(e => e.Server.HostName.Equals(hostName, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(component))
                query = query.Where(e => e.Component.Equals(component, StringComparison.OrdinalIgnoreCase));

            return query.OrderByDescending(e => e.DateOccured).ToList();
        }

        public PaginatedResponse<AuditHistoryRecord> GetAuditHistory(DateTime? fromDate = null, DateTime? toDate = null, string? endpoint = null, string? ipAddress = null, string? method = null, string? status = null, int pageSize = 10, int pageNumber = 1)
        {
            var query = AuditHistory.AsEnumerable();

            if (fromDate.HasValue)
                query = query.Where(a => a.OccuredAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.OccuredAt <= toDate.Value.AddDays(1));

            if (!string.IsNullOrEmpty(endpoint))
                query = query.Where(a => a.Endpoint.Contains(endpoint, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(ipAddress))
                query = query.Where(a => a.IPAddress.Contains(ipAddress, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(method) && method != "All")
                query = query.Where(a => a.Method.Equals(method, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(status) && status != "All")
                query = query.Where(a => a.Status.StartsWith(status, StringComparison.OrdinalIgnoreCase));

            var filtered = query.ToList();
            var totalCount = filtered.Count;
            var entries = filtered.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedResponse<AuditHistoryRecord>
            {
                Entries = entries,
                EntryCount = entries.Count,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPageCount = (int)Math.Ceiling((double)totalCount / pageSize),
                TotalCount = totalCount
            };
        }

        public AuditHistoryRecord? GetAuditHistoryRecord(int id) => AuditHistory.FirstOrDefault(a => a.Id == id);

        public List<AuditHistoryRecord> GetRecentAuditHistory(int count = 10)
        {
            return AuditHistory.OrderByDescending(a => a.OccuredAt).Take(count).ToList();
        }

        public List<ErrorLogRecord> GetErrors(DateTime? fromDate = null, string? ipAddress = null, string? summary = null)
        {
            var query = ErrorLogs.AsEnumerable();

            if (fromDate.HasValue)
                query = query.Where(e => e.DateOccured >= fromDate.Value);

            if (!string.IsNullOrEmpty(ipAddress))
                query = query.Where(e => e.IPAddress.Contains(ipAddress, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(summary))
                query = query.Where(e => e.Summary.Contains(summary, StringComparison.OrdinalIgnoreCase));

            return query.ToList();
        }

        public ErrorLogRecord? GetError(int id) => ErrorLogs.FirstOrDefault(e => e.Id == id);
    }
}
