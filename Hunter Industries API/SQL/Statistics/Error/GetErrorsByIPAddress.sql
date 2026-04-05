select
	IPAddress,
	count(*) as ErrorCount
from ErrorLog with (nolock)
where DateOccured >= DATEADD(DAY, -30, GETUTCDATE())
group by IPAddress
order by ErrorCount desc