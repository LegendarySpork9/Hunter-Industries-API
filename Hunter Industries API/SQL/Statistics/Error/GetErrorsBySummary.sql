select
	Summary,
	count(*) as ErrorCount
from ErrorLog with (nolock)
where DateOccured >= datefromparts(year(getutcdate()), month(getutcdate()), 1)
and DateOccured < dateadd(day, 1, eomonth(getutcdate()))
group by Summary
order by ErrorCount desc