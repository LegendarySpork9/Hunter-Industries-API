insert into ComponentInformation (ServerInformationId, ComponentId, ComponentStatusId, DateOccured)
output inserted.ComponentInformationId
values (
	@ServerId,
	(
		select
			ComponentId
		from Component with (nolock)
		where [Name] = @Component
	),
	(
		select
			ComponentStatusId
		from ComponentStatus with (nolock)
		where [Value] = @Status
	),
	GETDATE()
)