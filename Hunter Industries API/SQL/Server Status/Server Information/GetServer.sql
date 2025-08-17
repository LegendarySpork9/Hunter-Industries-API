select ServerInformationID from ServerInformation with (nolock)
join Machine with (nolock) on ServerInformation.MachineID = Machine.MachineID
join Game with (nolock) on ServerInformation.GameID = Game.GameID
where HostName = @HostName
and [Name] = @Game
and [Version] = @GameVersion