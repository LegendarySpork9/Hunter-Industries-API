update [Connection] set
	IPAddress = @IPAddress,
	[Port] = @Port
where ConnectionId = @ConnectionId