update [Connection] set
	IPAddress = @ipAddress,
	[Port] = @port
where ConnectionId = @connectionId