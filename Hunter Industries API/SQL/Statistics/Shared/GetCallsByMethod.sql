select
	[Value],
	count(*) as MethodCalls
from AuditHistory with (nolock)
join Method with (nolock) on AuditHistory.MethodId = Method.MethodId
where DateOccured >= dateadd(day, -30, getutcdate())