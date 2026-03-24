select
	ErrorId,
	DateOccured,
	IPAddress,
	Summary,
	[Message]
from ErrorLog with (nolock)
where ErrorId is not null