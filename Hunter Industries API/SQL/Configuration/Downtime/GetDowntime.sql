select
	DowntimeId,
	[Time],
	Duration,
	IsDeleted
from Downtime with (nolock)