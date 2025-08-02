select ServerInformationID, HostName, [Name], GameVersion, IPAddress, [Port], IsActive from ServerInformation with (nolock)
join Machine with (nolock) on ServerInformation.MachineID = Machine.MachineID
join Game with (nolock) on ServerInformation.GameID = Game.GameID