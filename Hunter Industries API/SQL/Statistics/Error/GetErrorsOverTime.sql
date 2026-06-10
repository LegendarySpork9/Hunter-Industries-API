with Dates as (
    select datefromparts(year(dateadd(month, -11, getutcdate())), month(dateadd(month, -11, getutcdate())), 1) as DateOccured
    union all
    select dateadd(month, 1, DateOccured)
    from Dates
    where DateOccured < datefromparts(year(getutcdate()), month(getutcdate()), 1)
)
select
	format(DateOccured, 'MMM') as [Month],
	isnull(ErrorData.Errors, 0) as Errors
from Dates
left join (
    select
	    format(DateOccured, 'MMM') as [Month],
	    count(*) as Errors
    from ErrorLog with (nolock)
    where DateOccured >= dateadd(year, -1, getutcdate())
    group by format(DateOccured, 'MMM')
) as ErrorData on format(DateOccured, 'MMM') = ErrorData.[Month]
order by Dates.DateOccured asc
option (maxrecursion 12)