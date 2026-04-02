insert into Authorisation (Phrase)
output inserted.PhraseId
values (
	@phrase
)