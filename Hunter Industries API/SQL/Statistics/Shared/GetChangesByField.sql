select
	Field,
	count(*) as ChangeCount
from [Change] with (nolock)
join AuditHistory with (nolock) on [Change].AuditId = AuditHistory.AuditId
where DateOccured >= DATEADD(DAY, -30, GETUTCDATE())
/* Move to Service

group by Field
order by ChangeCount desc*