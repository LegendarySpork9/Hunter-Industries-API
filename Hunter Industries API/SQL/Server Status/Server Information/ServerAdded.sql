insert into ServerInformation ([Name], MachineId, GameId, ConnectionId, DowntimeId)
output inserted.ServerInformationId
select
	@name,
	M.MachineId,
	G.GameId,
	C.ConnectionId,
	D.DowntimeId
from Machine M with (nolock)
join Game G with (nolock) on G.[Name] = @game
	and G.[Version] = @gameVersion
join Connection C with (nolock) on C.IPAddress = @ipAddress
	and C.[Port] = @port
left join Downtime D with (nolock) on D.[Time] = @time
	and D.Duration = @duration
where M.HostName = @hostName