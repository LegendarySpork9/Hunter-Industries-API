select
	[Value],
	count(*) as EndpointCalls
from AuditHistory with (nolock)
join [Endpoint] with (nolock) on AuditHistory.EndpointId = [Endpoint].EndpointId
where DateOccured >= dateadd(day, -30, getutcdate())
/* Move to Service

group by [Value]
order by EndpointCalls desc*/