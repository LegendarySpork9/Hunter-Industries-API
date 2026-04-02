insert into Component ([Name])
output inserted.ComponentId
values (
	@Name
)