select
	ServerInformationID,
	HostName,
	[Name],
	[Version],
	IPAddress,
	[Port],
	[Time],
	IsActive
from ServerInformation with (nolock)
join Machine with (nolock) on ServerInformation.MachineID = Machine.MachineID
join Game with (nolock) on ServerInformation.GameID = Game.GameID
join Connection with (nolock) on ServerInformation.ConnectionID = Connection.ConnectionID
left join Downtime with (nolock) on ServerInformation.DowntimeID = Downtime.DowntimeID