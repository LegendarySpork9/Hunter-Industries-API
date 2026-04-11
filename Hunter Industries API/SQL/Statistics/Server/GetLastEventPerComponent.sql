select
	C.[Name] as Component,
	CS.[Value] as [Status],
	CI.DateOccured
from ComponentInformation CI with (nolock)
join Component C with (nolock) on CI.ComponentId = C.ComponentId
join ComponentStatus CS with (nolock) on CI.ComponentStatusId = CS.ComponentStatusId
where CI.ServerInformationId = @serverId
and CI.DateOccured = (
	select max(CI2.DateOccured)
	from ComponentInformation CI2 with (nolock)
	where CI2.ServerInformationId = CI.ServerInformationId
	and CI2.ComponentId = CI.ComponentId
)
order by CI.DateOccured desc