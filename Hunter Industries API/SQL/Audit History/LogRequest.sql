insert into AuditHistory (IPAddress, EndpointID, MethodID, StatusID, DateOccured, [Parameters])
output inserted.AuditID
values (
	@IPAddress,
	@EndpointID,
	@MethodID,
	@StatusID,
	GETDATE(),
	@Parameters
)