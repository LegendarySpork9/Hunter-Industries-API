insert into [User] (Name)
output inserted.UserId
values (
	@Name
)