select distinct
	format(DateOccured, 'MMM dd') as [Day],
	SuccessfulCalls.CallCount as SuccessfulCalls,
	UnsuccessfulCalls.CallCount as UnsuccessfulCalls
from AuditHistory with (nolock)
join (
	select
		format(DateOccured, 'MMM dd') as [Day],
		count (*) as CallCount
	from AuditHistory with (nolock)
	join StatusCode with (nolock) on AuditHistory.StatusId = StatusCode.StatusId
	where DateOccured >= datefromparts(year(getutcdate()), month(getutcdate()), 1)
	and DateOccured < dateadd(day, 1, eomonth(getutcdate()))
	and [Value] like '20%'
	group by format(DateOccured, 'MMM dd')
) as SuccessfulCalls on format(DateOccured, 'MMM dd') = SuccessfulCalls.[Day]
join (
	select
		format(DateOccured, 'MMM dd') as [Day],
		count (*) as CallCount
	from AuditHistory with (nolock)
	join StatusCode with (nolock) on AuditHistory.StatusId = StatusCode.StatusId
	where DateOccured >= datefromparts(year(getutcdate()), month(getutcdate()), 1)
	and DateOccured < dateadd(day, 1, eomonth(getutcdate()))
	and [Value] not like '20%'
	group by format(DateOccured, 'MMM dd')
) as UnsuccessfulCalls on format(DateOccured, 'MMM dd') = UnsuccessfulCalls.[Day]
order by [Day] asc
