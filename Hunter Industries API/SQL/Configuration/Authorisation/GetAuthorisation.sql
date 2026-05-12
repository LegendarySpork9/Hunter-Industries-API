select
	Authorisation.PhraseId,
	Phrase,
	Authorisation.IsDeleted
from Authorisation with (nolock)