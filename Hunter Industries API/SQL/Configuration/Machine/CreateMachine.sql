insert into Machine (HostName)
output inserted.MachineId
values (
	@hostName
)