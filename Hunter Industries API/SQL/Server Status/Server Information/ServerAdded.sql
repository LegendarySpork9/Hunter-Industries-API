insert into ServerInformation (MachineID, GameID, GameVersion, IPAddress)
output inserted.ServerInformationID
values ((select MachineID from Machine where HostName = @HostName), (select GameID from Game where [Name] = @Game), @GameVersion, @IPAddress)