select
	ApplicationSettingId
from ApplicationSetting with (nolock)
where ApplicationId = @ApplicationId
and [Name] = @Name