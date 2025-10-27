select
	[Application].[Name],
	UserSettingsID,
	UserSetting.[Name],
	[Value]
from UserSetting with (nolock)
join [Application] with (nolock) on UserSetting.ApplicationID = [Application].ApplicationID
where UserSettingsID is not null