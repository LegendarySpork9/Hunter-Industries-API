select count(*) from AuditHistory AH with (nolock)
join [Endpoint] E with (nolock) on AH.EndpointId = E.EndpointId
left join APIUser AU with (nolock) on AH.UserId = AU.UserId
left join [Application] App with (nolock) on AH.ApplicationId = App.ApplicationId
where AH.AuditId is not null