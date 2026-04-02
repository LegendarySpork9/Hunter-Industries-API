insert into ComponentInformation (ServerInformationId, ComponentId, ComponentStatusId, DateOccured)
output inserted.ComponentInformationId
select
	@serverId,
	C.ComponentId,
	CS.ComponentStatusId,
	GETUTCDATE()
from Component C with (nolock)
join ComponentStatus CS with (nolock) on CS.[Value] = @status
where C.[Name] = @component