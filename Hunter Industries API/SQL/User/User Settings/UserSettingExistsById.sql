select
	UserSettingsID
from UserSetting with (nolock)
where UserSettingsID = @Id