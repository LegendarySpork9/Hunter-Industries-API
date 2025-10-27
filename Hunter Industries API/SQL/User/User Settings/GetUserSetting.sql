select
	UserSettingsID,
	UserSetting.[Name],
	[Value]
from UserSetting with (nolock)
where UserSettingsID = @Id