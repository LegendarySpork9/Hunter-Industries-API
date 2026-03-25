select
	DowntimeId
from Downtime with (nolock)
where DowntimeId = @DowntimeId