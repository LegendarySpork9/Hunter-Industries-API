select AH.AuditID, IPAddress, E.Value, M.Value, SC.Value, AH.DateOccured, [Parameters], AttemptID, Username, Phrase, IsSuccessful, ChangeID, CE.Value, Field, OldValue, NewValue from AuditHistory AH with (nolock)
join [Endpoint] E with (nolock) on AH.EndpointID = E.EndpointID
join Method M with (nolock) on AH.MethodID = M.MethodID
join StatusCode SC with (nolock) on AH.StatusID = SC.StatusID
left join LoginAttempt L with (nolock) on AH.AuditID = L.AuditID
left join APIUser AU with (nolock) on L.UserID = AU.UserID
left join Authorisation A with (nolock) on L.PhraseID = A.PhraseID
left join [Change] C with (nolock) on AH.AuditID = C.AuditID
left join [Endpoint] CE with (nolock) on C.EndpointID = CE.EndpointID
where AH.AuditID is not null