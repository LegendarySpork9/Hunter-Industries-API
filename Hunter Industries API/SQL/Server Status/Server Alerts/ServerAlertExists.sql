select
	ServerAlertsID
from ServerAlert with (nolock)
where ServerAlertsID = @AlertID