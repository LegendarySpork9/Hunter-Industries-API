select
	DowntimeId,
	[Time]
from Downtime with (nolock)
order by DowntimeId asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only