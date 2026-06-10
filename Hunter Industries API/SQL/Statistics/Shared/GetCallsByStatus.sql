select
	[Value],
	count(*) as StatusCalls
from AuditHistory with (nolock)
join StatusCode with (nolock) on AuditHistory.StatusId = StatusCode.StatusId
where DateOccured >= datefromparts(year(getutcdate()), month(getutcdate()), 1)
and DateOccured < dateadd(day, 1, eomonth(getutcdate()))