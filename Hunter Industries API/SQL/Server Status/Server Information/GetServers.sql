select
	ServerInformationId,
	HostName,
	[Name],
	[Version],
	IPAddress,
	[Port],
	[Time],
	IsActive
from ServerInformation with (nolock)
join Machine with (nolock) on ServerInformation.MachineId = Machine.MachineId
join Game with (nolock) on ServerInformation.GameId = Game.GameId
join Connection with (nolock) on ServerInformation.ConnectionId = Connection.ConnectionId
left join Downtime with (nolock) on ServerInformation.DowntimeId = Downtime.DowntimeId