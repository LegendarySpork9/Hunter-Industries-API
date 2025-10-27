select
	UserSettingId,
	UserSetting.[Name],
	[Value]
from UserSetting with (nolock)
where UserSettingId = @Id