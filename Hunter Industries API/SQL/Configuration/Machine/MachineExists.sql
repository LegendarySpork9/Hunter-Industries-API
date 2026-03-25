select
	MachineId
from Machine with (nolock)
where MachineId = @MachineId