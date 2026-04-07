select
	IPAddress,
	count(*) as ErrorCount
from ErrorLog with (nolock)
where DateOccured >= dateadd(day, -30, getutcdate())
group by IPAddress
order by ErrorCount desc