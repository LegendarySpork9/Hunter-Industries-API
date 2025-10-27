insert into AuditHistory (IPAddress, EndpointId, MethodId, StatusId, DateOccured, [Parameters])
output inserted.AuditId
values (
	@IPAddress,
	@EndpointId,
	@MethodId,
	@StatusId,
	GETDATE(),
	@Parameters
)