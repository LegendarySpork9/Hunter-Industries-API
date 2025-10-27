insert into APIUser (Username, [Password])
output inserted.UserID
values (
	@Username,
	@Password
)