select
	GameId
from Game with (nolock)
where [Name] = @Name
and [Version] = @Version