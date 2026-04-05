select
(
	select
		count(*) as CallCount
	from AuditHistory with (nolock)
	join StatusCode with (nolock) on AuditHistory.StatusId = StatusCode.StatusId
	where DateOccured >= DATEADD(DAY, -30, GETUTCDATE())
	and [Value] like '20%'
) as SuccessfulCalls,
(
	select
		count(*) as CallCount
	from AuditHistory with (nolock)
	join StatusCode with (nolock) on AuditHistory.StatusId = StatusCode.StatusId
	where DateOccured >= DATEADD(DAY, -30, GETUTCDATE())
	and [Value] not like '20%'
) as UnsuccessfulCalls