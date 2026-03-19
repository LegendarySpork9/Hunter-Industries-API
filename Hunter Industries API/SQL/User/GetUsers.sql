select UserId, Username, [Password] from APIUser with (nolock)
where IsDeleted = 0