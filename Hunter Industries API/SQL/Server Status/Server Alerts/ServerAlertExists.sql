select
	ServerAlertId
from ServerAlert with (nolock)
join Component with (nolock) on ServerAlert.ComponentId = Component.ComponentId
join ServerAlertStatus with (nolock) on ServerAlert.AlertStatusId = ServerAlertStatus.AlertStatusId