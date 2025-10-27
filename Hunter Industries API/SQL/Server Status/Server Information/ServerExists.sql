select
	ServerInformationId
from ServerInformation with (nolock)
join Machine with (nolock) on ServerInformation.MachineId = Machine.MachineId
join Game with (nolock) on ServerInformation.GameId = Game.GameId
where HostName = @HostName
and [Name] = @Game
and [Version] = @GameVersion