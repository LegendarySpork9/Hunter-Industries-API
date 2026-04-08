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
	where DateOccured >= datefromparts(year(getutcdate()), month(getutcdate()), 1)
	and DateOccured < dateadd(day, 1, eomonth(getutcdate()))
) as CallCount,
(
	select
		count(*) as PreviousCallCount
	from AuditHistory with (nolock)
	where DateOccured >= dateadd(month, -1, datefromparts(year(getutcdate()), month(getutcdate()), 1))
	and DateOccured < datefromparts(year(getutcdate()), month(getutcdate()), 1)
) as PreviousCallCount,
(
	select
		count(*) as LoginAttemptCount
	from LoginAttempt with (nolock)
	where DateOccured >= datefromparts(year(getutcdate()), month(getutcdate()), 1)
	and DateOccured < dateadd(day, 1, eomonth(getutcdate()))
) as LoginAttemptCount,
(
	select
		count(*) as PreviousLoginAttemptCount
	from LoginAttempt with (nolock)
	where DateOccured >= dateadd(month, -1, datefromparts(year(getutcdate()), month(getutcdate()), 1))
	and DateOccured < datefromparts(year(getutcdate()), month(getutcdate()), 1)
) as PreviousLoginAttemptCount,
(
	select
		count(*) as ChangeCount
	from [Change] with (nolock)
	join AuditHistory with (nolock) on [Change].AuditId = AuditHistory.AuditId
	where DateOccured >= datefromparts(year(getutcdate()), month(getutcdate()), 1)
	and DateOccured < dateadd(day, 1, eomonth(getutcdate()))
) as ChangeCount,
(
	select
		count(*) as PreviousChangeCount
	from [Change] with (nolock)
	join AuditHistory with (nolock) on [Change].AuditId = AuditHistory.AuditId
	where DateOccured >= dateadd(month, -1, datefromparts(year(getutcdate()), month(getutcdate()), 1))
	and DateOccured < datefromparts(year(getutcdate()), month(getutcdate()), 1)
) as PreviousChangeCount,
(
	select
		count(*) as ErrorCount
	from ErrorLog with (nolock)
	where DateOccured >= datefromparts(year(getutcdate()), month(getutcdate()), 1)
	and DateOccured < dateadd(day, 1, eomonth(getutcdate()))
) as ErrorCount,
(
	select
		count(*) as PreviousErrorCount
	from ErrorLog with (nolock)
	where DateOccured >= dateadd(month, -1, datefromparts(year(getutcdate()), month(getutcdate()), 1))
	and DateOccured < datefromparts(year(getutcdate()), month(getutcdate()), 1)
) as PreviousErrorCount
