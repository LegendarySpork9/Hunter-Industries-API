select
	ServerInformationId,
	ServerInformation.[Name] as ServerName,
	HostName,
	Game.[Name] as Game,
	[Version],
	IPAddress,
	[Port],
	[Time],
	Duration,
	IsActive
from ServerInformation with (nolock)
join Machine with (nolock) on ServerInformation.MachineId = Machine.MachineId
join Game with (nolock) on ServerInformation.GameId = Game.GameId
join [Connection] with (nolock) on ServerInformation.ConnectionId = [Connection].ConnectionId
left join Downtime with (nolock) on ServerInformation.DowntimeId = Downtime.DowntimeId