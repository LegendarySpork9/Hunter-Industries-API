select
	GameId,
	[Name],
	[Version]
from Game with (nolock)
order by GameId asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only