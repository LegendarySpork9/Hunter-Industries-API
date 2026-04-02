update Game set
	[Name] = @name,
	[Version] = @version
where GameId = @gameId