select
	[Value],
	count(*) as MethodCalls
from AuditHistory with (nolock)
join Method with (nolock) on AuditHistory.MethodId = Method.MethodId
where DateOccured >= DATEADD(DAY, -30, GETUTCDATE())
/* Move to Service

group by [Value]
order by MethodCalls desc*/