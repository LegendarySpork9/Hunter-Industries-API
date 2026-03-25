select
	ComponentId,
	[Name]
from Component with (nolock)
order by ComponentId asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only