insert into APIUser (Username, [Password])
output inserted.UserId
values (
	@Username,
	@Password
)