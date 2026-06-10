select
	ChangeId,
	Field,
	OldValue,
	NewValue
from [Change] with (nolock)
where AuditId = @auditId