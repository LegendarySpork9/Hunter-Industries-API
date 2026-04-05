select distinct
	format(DateOccured, 'yy MMM') as [Month],
	SuccessfulCalls.CallCount as SuccessfulCalls,
	UnsuccessfulCalls.CallCount as UnsuccessfulCalls
from AuditHistory with (nolock)
join (
	select
		format(DateOccured, 'yy MMM') as [Month],
		count (*) as CallCount
	from AuditHistory with (nolock)
	join StatusCode with (nolock) on AuditHistory.StatusId = StatusCode.StatusId
	where DateOccured >= DATEADD(DAY, -30, GETUTCDATE())
	and [Value] like '20%'
	group by format(DateOccured, 'yy MMM')
) as SuccessfulCalls on format(DateOccured, 'yy MMM') = SuccessfulCalls.[Month]
join (
	select
		format(DateOccured, 'yy MMM') as [Month],
		count (*) as CallCount
	from AuditHistory with (nolock)
	join StatusCode with (nolock) on AuditHistory.StatusId = StatusCode.StatusId
	where DateOccured >= DATEADD(DAY, -30, GETUTCDATE())
	and [Value] not like '20%'
	group by format(DateOccured, 'yy MMM')
) as UnsuccessfulCalls on format(DateOccured, 'yy MMM') = UnsuccessfulCalls.[Month]
order by [Month] asc