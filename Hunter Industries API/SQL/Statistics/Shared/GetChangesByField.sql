select
	Field,
	count(*) as ChangeCount
from [Change] with (nolock)
join AuditHistory with (nolock) on [Change].AuditId = AuditHistory.AuditId
where DateOccured >= datefromparts(year(getutcdate()), month(getutcdate()), 1)
and DateOccured < dateadd(day, 1, eomonth(getutcdate()))