select
	ServerAlertId
from ServerAlert with (nolock)
where ServerAlertId = @AlertId