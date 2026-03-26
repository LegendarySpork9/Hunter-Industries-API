insert into [Connection] (IPAddress, [Port])
output inserted.ConnectionId
values (
	@IPAddress,
	@Port
)