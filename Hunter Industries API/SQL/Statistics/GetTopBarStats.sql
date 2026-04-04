select
(
	select
		count(*) as ApplicationCount
	from [Application] with (nolock)
	where IsDeleted = 0
) as ActiveApplicationCount,
(
	select
		count(*) as UserCount
	from APIUser with (nolock)
	where IsDeleted = 0
) as ActiveUserCount,
(
	select
		count(*) as CallCount
	from AuditHistory with (nolock)
	where DateOccured >= DATEADD(DAY, -30, GETUTCDATE())
) as CallCount,
(
	select
		count(*) as LoginAttemptCount
	from LoginAttempt with (nolock)
	where DateOccured >= DATEADD(DAY, -30, GETUTCDATE())
) as LoginAttemptCount,
(
	select
		count(*) as ChangeCount
	from [Change] with (nolock)
	join AuditHistory with (nolock) on [Change].AuditId = AuditHistory.AuditId
	where DateOccured >= DATEADD(DAY, -30, GETUTCDATE())
) as ChangeCount,
(
	select
		count(*) as ErrorCount
	from ErrorLog with (nolock)
	where DateOccured >= DATEADD(DAY, -30, GETUTCDATE())
) as ErrorCount