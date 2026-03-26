select
	MachineId
from Machine with (nolock)
where HostName = @HostName