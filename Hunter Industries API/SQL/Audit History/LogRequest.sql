insert into AuditHistory (IPAddress, EndpointId, EndpointVersionId, MethodId, StatusId, DateOccured, [Parameters], UserId, ApplicationId)
output inserted.AuditId
select
	@IPAddress,
	@EndpointId,
	@EndpointVersionId,
	@MethodId,
	@StatusId,
	GETUTCDATE(),
	@Parameters,
	AU.UserId,
	A.ApplicationId
from (select 1 as Dummy) D
left join APIUser AU with (nolock) on AU.Username = @Username
left join [Application] A with (nolock) on A.[Name] = @ApplicationName