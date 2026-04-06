select
	Field,
	count(*) as ChangeCount
from [Change] with (nolock)
join AuditHistory with (nolock) on [Change].AuditId = AuditHistory.AuditId
where DateOccured >= dateadd(day, -30, getutcdate())