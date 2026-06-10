select
	UserId,
	Username,
	[Password],
	IsDeleted
from APIUser with (nolock)
where UserId = @userId