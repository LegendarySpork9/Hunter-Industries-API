insert into ServerInformation (MachineID, GameID, ConnectionID, DowntimeID)
output inserted.ServerInformationID
values (
	(
		select
			MachineID
		from Machine with (nolock)
		where HostName = @HostName
	),
	(
		select
			GameID
		from Game with (nolock)
		where [Name] = @Game
		and [Version] = @GameVersion
	),
	(
		select
			ConnectionID
		from Connection with (nolock)
		where IPAddress = @IPAddress
		and [Port] = @Port
	),
	(
		select
			DowntimeID
		from Downtime with (nolock)
		where [Time] = @Time
	)
)