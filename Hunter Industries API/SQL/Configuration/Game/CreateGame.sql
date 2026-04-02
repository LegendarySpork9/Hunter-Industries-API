insert into Game ([Name], [Version])
output inserted.GameId
values (
	@Name,
	@Version
)