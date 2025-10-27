insert into ComponentInformation (ServerInformationID, ComponentID, ComponentStatusID, DateOccured)
output inserted.ComponentInformationID
values (
	@ServerID,
	(
		select
			ComponentID
		from Component with (nolock)
		where [Name] = @Component
	),
	(
		select
			ComponentStatusID
		from ComponentStatus with (nolock)
		where [Value] = @Status
	),
	GETDATE()
)