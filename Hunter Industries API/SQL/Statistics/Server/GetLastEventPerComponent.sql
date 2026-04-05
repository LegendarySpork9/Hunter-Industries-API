select
	format(DateOccured, 'yy MMM') as [Month],
	count(*) as Errors
from ErrorLog with (nolock)
group by format(DateOccured, 'yy MMM')
order by [Month] asc