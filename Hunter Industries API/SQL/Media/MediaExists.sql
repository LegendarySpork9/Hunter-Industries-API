select
	MediaId
from Media with (nolock)
join [Application] with (nolock) on Media.ApplicationId = [Application].ApplicationId