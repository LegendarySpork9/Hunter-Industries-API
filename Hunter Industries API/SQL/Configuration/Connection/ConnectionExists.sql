select
	ConnectionId
from [Connection] with (nolock)
where IPAddress = @IPAddress
and [Port] = @Port