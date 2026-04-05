select
	Summary,
	count(*) as ErrorCount
from ErrorLog with (nolock)
where DateOccured >= DATEADD(DAY, -30, GETUTCDATE())
group by Summary
order by ErrorCount desc