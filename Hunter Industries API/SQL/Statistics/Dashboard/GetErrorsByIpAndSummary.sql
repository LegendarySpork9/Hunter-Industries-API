select
	IPAddress,
	Summary,
	count(*) as ErrorCount
from ErrorLog with (nolock)
where DateOccured >= DATEADD(DAY, -30, GETUTCDATE())
group by IPAddress, Summary
order by IPAddress asc, ErrorCount desc