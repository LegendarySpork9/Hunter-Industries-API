select
	Summary,
	count(*) as ErrorCount
from ErrorLog with (nolock)
where DateOccured >= dateadd(day, -30, getutcdate())
group by Summary
order by ErrorCount desc