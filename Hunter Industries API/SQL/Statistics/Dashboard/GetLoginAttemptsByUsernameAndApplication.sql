select distinct
	isnull(APIUser.Username, 'Unknown') as Username,
	isnull([Name], 'Unknown') as [Application],
	isnull(Successful.Attempts, 0) as SuccessfulAttempts,
	isnull(Unsuccessful.Attempts, 0) as UnsccessfulAttempts,
	isnull(Successful.Attempts, 0) + isnull(Unsuccessful.Attempts, 0) as TotalAttempts
from LoginAttempt with (nolock)
left join APIUser with (nolock) on LoginAttempt.UserId = APIUser.UserId
left join Authorisation with (nolock) on LoginAttempt.PhraseId = Authorisation.PhraseId
left join [Application] with (nolock) on Authorisation.PhraseId = [Application].PhraseId
left join (
	select
		isnull(Username, 'Unknown') as Username,
		isnull([Name], 'Unknown') as [Application],
		count(*) as Attempts
	from LoginAttempt LAS with (nolock)
	left join APIUser AUS with (nolock) on LAS.UserId = AUS.UserId
	left join Authorisation AuthS with (nolock) on LAS.PhraseId = AuthS.PhraseId
	left join [Application] AppS with (nolock) on AuthS.PhraseId = AppS.PhraseId
	where DateOccured >= datefromparts(year(getutcdate()), month(getutcdate()), 1)
	and DateOccured < dateadd(day, 1, eomonth(getutcdate()))
	and IsSuccessful = 1
	group by Username, [Name]
) Successful on isnull(APIUser.Username, 'Unknown') = Successful.Username and isnull([Name], 'Unknown') = Successful.[Application]
left join (
	select
		isnull(Username, 'Unknown') as Username,
		isnull([Name], 'Unknown') as [Application],
		count(*) as Attempts
	from LoginAttempt LAS with (nolock)
	left join APIUser AUS with (nolock) on LAS.UserId = AUS.UserId
	left join Authorisation AuthS with (nolock) on LAS.PhraseId = AuthS.PhraseId
	left join [Application] AppS with (nolock) on AuthS.PhraseId = AppS.PhraseId
	where DateOccured >= datefromparts(year(getutcdate()), month(getutcdate()), 1)
	and DateOccured < dateadd(day, 1, eomonth(getutcdate()))
	and IsSuccessful = 0
	group by Username, [Name]
) Unsuccessful on isnull(APIUser.Username, 'Unknown') = Unsuccessful.Username and isnull([Name], 'Unknown') = Unsuccessful.[Application]
where isnull(Successful.Attempts, 0) + isnull(Unsuccessful.Attempts, 0) > 0
order by Username asc, TotalAttempts desc
