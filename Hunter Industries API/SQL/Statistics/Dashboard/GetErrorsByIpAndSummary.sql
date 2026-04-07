select
	IPAddress,
	Summary,
	count(*) as ErrorCount
from ErrorLog with (nolock)
where DateOccured >= dateadd(day, -30, getutcdate())
group by IPAddress, Summary
order by IPAddress asc, ErrorCount desc