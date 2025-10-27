select
	[Name]
from Application with (nolock)
join Authorisation with (nolock) on Application.PhraseId = Authorisation.PhraseId
where Phrase = @Phrase