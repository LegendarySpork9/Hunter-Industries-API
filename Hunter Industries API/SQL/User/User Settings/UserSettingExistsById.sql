select
	UserSettingId
from UserSetting with (nolock)
where UserSettingId = @id