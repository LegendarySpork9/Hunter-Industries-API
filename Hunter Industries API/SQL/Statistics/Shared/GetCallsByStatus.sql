select
	[Value],
	count(*) as StatusCalls
from AuditHistory with (nolock)
join StatusCode with (nolock) on AuditHistory.StatusId = StatusCode.StatusId
where DateOccured >= DATEADD(DAY, -30, GETUTCDATE())
/* Move to Service

group by [Value]
order by StatusCalls desc*/