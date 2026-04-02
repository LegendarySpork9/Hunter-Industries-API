insert into [Application] (PhraseId, [Name])
output inserted.ApplicationId
select
	Authorisation.PhraseId,
	@Name
from Authorisation with (nolock)
where Phrase = @Phrase