select
	ServerAlertId
from ServerAlert with (nolock)
where ServerAlertId = @alertId