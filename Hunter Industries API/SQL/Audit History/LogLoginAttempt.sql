insert into LoginAttempt (UserID, PhraseID, AuditID, DateOccured, IsSuccessful)
values (
	(
		select
			UserID
		from APIUser with (nolock)
		where Username = @Username
		and [Password] = @Password
	),
	(
		select
			PhraseID
		from Authorisation with (nolock)
		where Phrase = @Phrase
	),
	@AuditID,
	GETDATE(),
	@IsSuccessful
)