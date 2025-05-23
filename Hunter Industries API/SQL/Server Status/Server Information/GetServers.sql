select ServerInformationID, HostName, [Name], GameVersion, IPAddress, IsActive from ServerInformation with (nolock)
join Machine with (nolock) on ServerInformation.MachineID = Machine.MachineID
join Game with (nolock) on ServerInformation.GameID = Game.GameID