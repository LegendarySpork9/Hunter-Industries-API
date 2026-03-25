select
	PhraseId,
	Phrase
from Authorisation with (nolock)
order by PhraseId asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only