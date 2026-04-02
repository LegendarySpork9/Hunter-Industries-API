insert into Machine (HostName)
output inserted.MachineId
values (
	@HostName
)