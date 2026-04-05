select
	isnull(Username, 'Unknown') as Username,
	isnull([Name], 'Unknown') as [Application],
	count(*) as Attempts
from LoginAttempt with (nolock)
left join APIUser with (nolock) on LoginAttempt.UserId = APIUser.UserId
left join Authorisation with (nolock) on LoginAttempt.PhraseId = Authorisation.PhraseId
left join [Application] with (nolock) on Authorisation.PhraseId = [Application].PhraseId
where DateOccured >= DATEADD(DAY, -30, GETUTCDATE())
group by Username, [Name]
order by Username asc, Attempts desc