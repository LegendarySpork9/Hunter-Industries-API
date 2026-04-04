select
	Field,
	count(*) as ChangeCount
from [Change] with (nolock)
join AuditHistory with (nolock) on [Change].AuditId = AuditHistory.AuditId
where DateOccured >= DATEADD(DAY, -30, GETUTCDATE())
group by Field
order by ChangeCount desc