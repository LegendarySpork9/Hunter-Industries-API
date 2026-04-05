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
	where DateOccured >= dateadd(day, -30, getutcdate())
	and [Value] like '20%'
	group by format(DateOccured, 'MMM dd')
) as SuccessfulCalls on format(DateOccured, 'MMM dd') = SuccessfulCalls.[Day]
join (
	select
		format(DateOccured, 'MMM dd') as [Day],
		count (*) as CallCount
	from AuditHistory with (nolock)
	join StatusCode with (nolock) on AuditHistory.StatusId = StatusCode.StatusId
	where DateOccured >= dateadd(day, -30, getutcdate())
	and [Value] not like '20%'
	group by format(DateOccured, 'MMM dd')
) as UnsuccessfulCalls on format(DateOccured, 'MMM dd') = UnsuccessfulCalls.[Day]
order by [Day] asc