insert into LoginAttempt (UserId, PhraseId, AuditId, DateOccured, IsSuccessful)
select
	AU.UserId,
	A.PhraseId,
	@auditId,
	GETUTCDATE(),
	@isSuccessful
from (select 1 as Dummy) D
left join APIUser AU with (nolock) on AU.Username = @username
	and AU.[Password] = @password
left join Authorisation A with (nolock) on A.Phrase = @phrase