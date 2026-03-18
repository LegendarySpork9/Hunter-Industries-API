insert into ComponentInformation (ServerInformationId, ComponentId, ComponentStatusId, DateOccured)
output inserted.ComponentInformationId
select
	@ServerId,
	C.ComponentId,
	CS.ComponentStatusId,
	GETUTCDATE()
from Component C with (nolock)
join ComponentStatus CS with (nolock) on CS.[Value] = @Status
where C.[Name] = @Component