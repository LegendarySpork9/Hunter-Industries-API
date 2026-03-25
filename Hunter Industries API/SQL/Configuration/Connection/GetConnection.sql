select
	ConnectionId,
	IPAddress,
	[Port]
from [Connection] with (nolock)
order by ConnectionId asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only