select
	DowntimeId
from Downtime with (nolock)
where Time = @Time