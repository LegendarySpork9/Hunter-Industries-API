select
	format(DateOccured, 'yy MMM') as [Month],
	count(*) as Errors
from ErrorLog with (nolock)
where DateOccured >= dateadd(year, -1, getutcdate())
group by format(DateOccured, 'yy MMM')
order by [Month] asc