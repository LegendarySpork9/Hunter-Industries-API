select
	MachineId,
	HostName
from Machine with (nolock)
order by MachineId asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only