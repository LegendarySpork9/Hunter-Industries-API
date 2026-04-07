select top 5
	[Name],
	[Value],
	DateOccured
from ComponentInformation with (nolock)
join Component with (nolock) on ComponentInformation.ComponentId = Component.ComponentId
join ComponentStatus with (nolock) on ComponentInformation.ComponentStatusId = ComponentStatus.ComponentStatusId
where ServerInformationId = @serverId
order by DateOccured desc