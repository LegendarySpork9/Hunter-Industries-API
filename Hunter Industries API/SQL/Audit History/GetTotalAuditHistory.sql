select count(*) from AuditHistory AH with (nolock)
join [Endpoint] E with (nolock) on AH.EndpointId = E.EndpointId
where AH.AuditId is not null