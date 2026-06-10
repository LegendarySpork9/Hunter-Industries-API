select
	[Value],
	count(*) as MethodCalls
from AuditHistory with (nolock)
join Method with (nolock) on AuditHistory.MethodId = Method.MethodId
where DateOccured >= datefromparts(year(getutcdate()), month(getutcdate()), 1)
and DateOccured < dateadd(day, 1, eomonth(getutcdate()))