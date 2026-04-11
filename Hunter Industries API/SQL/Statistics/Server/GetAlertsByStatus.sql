select
	[Value],
	count(*) as Alerts
from ServerAlert with (nolock)
join ServerAlertStatus with (nolock) on ServerAlert.AlertStatusId = ServerAlertStatus.AlertStatusId
where ServerInformationId = @serverId
group by [Value]
order by Alerts desc