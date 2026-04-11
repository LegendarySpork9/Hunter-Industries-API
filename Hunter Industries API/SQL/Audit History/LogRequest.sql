insert into AuditHistory (IPAddress, EndpointId, EndpointVersionId, MethodId, StatusId, DateOccured, [Parameters], UserId, ApplicationId)
output inserted.AuditId
select
	@ipAddress,
	@endpointId,
	@endpointVersionId,
	@methodId,
	@statusId,
	GETUTCDATE(),
	@parameters,
	AU.UserId,
	A.ApplicationId
from (select 1 as Dummy) D
left join APIUser AU with (nolock) on AU.Username = @username
left join [Application] A with (nolock) on A.[Name] = @applicationName