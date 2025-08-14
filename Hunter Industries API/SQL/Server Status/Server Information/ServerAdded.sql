insert into ServerInformation (MachineID, GameID, ConnectionID, DowntimeID)
output inserted.ServerInformationID
values ((select MachineID from Machine where HostName = @HostName), (select GameID from Game where [Name] = @Game and [Version] = @GameVersion), (select ConnectionID from Connection where IPAddress = @IPAddress and [Port] = @Port), (select DowntimeID from Dowmtime where [Time] = @Time))