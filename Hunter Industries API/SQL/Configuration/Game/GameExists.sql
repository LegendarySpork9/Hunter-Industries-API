select
	GameId
from Game with (nolock)
where GameId = @GameId