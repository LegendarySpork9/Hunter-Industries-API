insert into ServerInformation (MachineId, GameId, ConnectionId, DowntimeId)
output inserted.ServerInformationId
values (
	(
		select
			MachineId
		from Machine with (nolock)
		where HostName = @HostName
	),
	(
		select
			GameId
		from Game with (nolock)
		where [Name] = @Game
		and [Version] = @GameVersion
	),
	(
		select
			ConnectionId
		from Connection with (nolock)
		where IPAddress = @IPAddress
		and [Port] = @Port
	),
	(
		select
			DowntimeId
		from Downtime with (nolock)
		where [Time] = @Time
	)
)