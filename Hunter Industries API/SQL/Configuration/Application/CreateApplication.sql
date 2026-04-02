insert into [Application] (PhraseId, [Name])
output inserted.ApplicationId
select
	Authorisation.PhraseId,
	@name
from Authorisation with (nolock)
where Phrase = @phrase