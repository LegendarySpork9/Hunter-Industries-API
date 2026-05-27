select
	AttemptId,
	AU.Username,
	A.Phrase,
	IsSuccessful
from LoginAttempt L with (nolock)
left join APIUser AU with (nolock) on L.UserId = AU.UserId
left join Authorisation A with (nolock) on L.PhraseId = A.PhraseId
where AuditId = @auditId