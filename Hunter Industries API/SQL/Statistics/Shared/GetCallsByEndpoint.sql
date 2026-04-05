select
	[Value],
	count(*) as EndpointCalls
from AuditHistory with (nolock)
join [Endpoint] with (nolock) on AuditHistory.EndpointId = [Endpoint].EndpointId
where DateOccured >= DATEADD(DAY, -30, GETUTCDATE())
/* Move to Service

group by [Value]
order by EndpointCalls desc*/