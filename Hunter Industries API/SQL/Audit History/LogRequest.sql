insert into AuditHistory (IPAddress, EndpointId, EndpointVersionId, MethodId, StatusId, DateOccured, [Parameters])
output inserted.AuditId
values (
	@IPAddress,
	@EndpointId,
	@EndpointVersionId,
	@MethodId,
	@StatusId,
	GETUTCDATE(),
	@Parameters
)