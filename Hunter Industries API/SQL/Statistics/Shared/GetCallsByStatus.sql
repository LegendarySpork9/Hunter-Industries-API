select
	[Value],
	count(*) as StatusCalls
from AuditHistory with (nolock)
join StatusCode with (nolock) on AuditHistory.StatusId = StatusCode.StatusId
where DateOccured >= dateadd(day, -30, getutcdate())
/* Move to Service

group by [Value]
order by StatusCalls desc*/