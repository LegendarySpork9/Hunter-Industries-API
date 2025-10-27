insert into LoginAttempt (UserId, PhraseId, AuditId, DateOccured, IsSuccessful)
values (
	(
		select
			UserId
		from APIUser with (nolock)
		where Username = @Username
		and [Password] = @Password
	),
	(
		select
			PhraseId
		from Authorisation with (nolock)
		where Phrase = @Phrase
	),
	@AuditId,
	GETDATE(),
	@IsSuccessful
)