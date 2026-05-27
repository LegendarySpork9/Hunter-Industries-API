select
	AH.AuditId,
	IPAddress,
	AU2.Username as AuditUsername,
	App.[Name] as ApplicationName,
	E.[Value] as [Endpoint],
	EV.[Value] as EndpointVersion,
	M.[Value] as [Method],
	SC.[Value] as StatusCode,
	AH.DateOccured,
	[Parameters],
	LEFT(AH.RequestBody, 10000) as RequestBody,
	LEFT(AH.ResponseBody, 10000) as ResponseBody
from AuditHistory AH with (nolock)
join [Endpoint] E with (nolock) on AH.EndpointId = E.EndpointId
join [EndpointVersion] EV with (nolock) on AH.EndpointVersionId = EV.EndpointVersionId
join Method M with (nolock) on AH.MethodId = M.MethodId
join StatusCode SC with (nolock) on AH.StatusId = SC.StatusId
left join APIUser AU2 with (nolock) on AH.UserId = AU2.UserId
left join [Application] App with (nolock) on AH.ApplicationId = App.ApplicationId
where AH.AuditID = @auditId