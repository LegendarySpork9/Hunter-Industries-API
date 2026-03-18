select
	[Application].[Name] as ApplicationName,
	UserSettingId,
	UserSetting.[Name] as SettingName,
	[Value]
from UserSetting with (nolock)
join [Application] with (nolock) on UserSetting.ApplicationId = [Application].ApplicationId
where UserSettingId is not null