select
	[Name],
	count(*) as Alerts
from ServerAlert with (nolock)
join Component with (nolock) on ServerAlert.ComponentId = Component.ComponentId
where ServerInformationId = @serverId
group by [Name]
order by Alerts desc