select
	Username,
	[Password],
	Salt
from APIUser with (nolock)
where IsDeleted = 0