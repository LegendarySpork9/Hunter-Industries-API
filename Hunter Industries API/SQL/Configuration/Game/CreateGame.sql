insert into Game ([Name], [Version])
output inserted.GameId
values (
	@name,
	@version
)