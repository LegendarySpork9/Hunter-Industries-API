select count(*) from AuditHistory AH with (nolock)
join [Endpoint] E with (nolock) on AH.EndpointID = E.EndpointID
join Method M with (nolock) on AH.MethodID = M.MethodID
join StatusCode SC with (nolock) on AH.StatusID = SC.StatusID
where AH.AuditID is not null