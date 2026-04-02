insert into LoginAttempt (UserId, PhraseId, AuditId, DateOccured, IsSuccessful)
select
	AU.UserId,
	A.PhraseId,
	@AuditId,
	GETUTCDATE(),
	@IsSuccessful
from (select 1 as Dummy) D
left join APIUser AU with (nolock) on AU.Username = @Username
	and AU.[Password] = @Password
left join Authorisation A with (nolock) on A.Phrase = @Phrase