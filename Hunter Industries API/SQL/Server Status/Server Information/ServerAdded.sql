insert into ServerInformation ([Name], MachineId, GameId, ConnectionId, DowntimeId)
output inserted.ServerInformationId
select
	@Name,
	M.MachineId,
	G.GameId,
	C.ConnectionId,
	D.DowntimeId
from Machine M with (nolock)
join Game G with (nolock) on G.[Name] = @Game
	and G.[Version] = @GameVersion
join Connection C with (nolock) on C.IPAddress = @IPAddress
	and C.[Port] = @Port
left join Downtime D with (nolock) on D.[Time] = @Time
	and D.Duration = @Duration
where M.HostName = @HostName