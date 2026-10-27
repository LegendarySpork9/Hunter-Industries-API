update APIUser set
	[Password] = @password,
	Salt = @salt
where Username = @username
and IsDeleted = 0