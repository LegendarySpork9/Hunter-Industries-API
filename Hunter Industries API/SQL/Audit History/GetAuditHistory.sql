select
	AH.AuditId,
	IPAddress,
	E.[Value] as Endpoint,
	M.[Value] as [Method],
	SC.[Value] as StatusCode,
	AH.DateOccured,
	[Parameters],
	AttemptId,
	Username,
	Phrase,
	IsSuccessful,
	ChangeId,
	CE.[Value] as ChangeEndpoint,
	Field,
	OldValue,
	NewValue
from AuditHistory AH with (nolock)
join [Endpoint] E with (nolock) on AH.EndpointId = E.EndpointId
join Method M with (nolock) on AH.MethodId = M.MethodId
join StatusCode SC with (nolock) on AH.StatusId = SC.StatusId
left join LoginAttempt L with (nolock) on AH.AuditId = L.AuditId
left join APIUser AU with (nolock) on L.UserId = AU.UserId
left join Authorisation A with (nolock) on L.PhraseId = A.PhraseId
left join [Change] C with (nolock) on AH.AuditId = C.AuditId
left join [Endpoint] CE with (nolock) on C.EndpointId = CE.EndpointId
where AH.AuditId is not null