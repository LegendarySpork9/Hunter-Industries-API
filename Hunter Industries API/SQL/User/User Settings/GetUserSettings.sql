select
	[Application].[Name],
	UserSettingId,
	UserSetting.[Name],
	[Value]
from UserSetting with (nolock)
join [Application] with (nolock) on UserSetting.ApplicationId = [Application].ApplicationId
where UserSettingId is not null