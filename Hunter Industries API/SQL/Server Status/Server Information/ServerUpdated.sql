update SI set
	[Name] = @name,
	EventInterval = @eventInterval,
	IsActive = @active,
	MachineId = M.MachineId,
	GameId = G.GameId,
	ConnectionId = C.ConnectionId,
	DowntimeId = D.DowntimeId
from ServerInformation SI
join Machine M with (nolock) on M.HostName = @hostName
join Game G with (nolock) on G.[Name] = @game
    and G.[Version] = @gameVersion
join [Connection] C with (nolock) on C.IPAddress = @ipAddress
    and C.[Port] = @port
left join Downtime D with (nolock) on D.[Time] = @time
    and D.Duration = @duration
where SI.ServerInformationId = @serverId