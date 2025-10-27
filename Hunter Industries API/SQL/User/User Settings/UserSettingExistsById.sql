select
	UserSettingsID
from UserSettings with (nolock)
where UserSettingsID = @Id