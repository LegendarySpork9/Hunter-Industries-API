select
	ServerInformationId
from ServerInformation with (nolock)
join Machine with (nolock) on ServerInformation.MachineId = Machine.MachineId
join Game with (nolock) on ServerInformation.GameId = Game.GameId
where ServerInformation.[Name] = @Name