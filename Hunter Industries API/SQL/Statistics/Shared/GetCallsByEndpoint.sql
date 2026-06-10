select
	[Value],
	count(*) as EndpointCalls
from AuditHistory with (nolock)
join [Endpoint] with (nolock) on AuditHistory.EndpointId = [Endpoint].EndpointId
where DateOccured >= datefromparts(year(getutcdate()), month(getutcdate()), 1)
and DateOccured < dateadd(day, 1, eomonth(getutcdate()))