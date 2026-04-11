insert into [Location] (HostName, IPAddress)
output inserted.LocationId
values (
	@hostname,
	@ipAddress
)