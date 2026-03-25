select
	ApplicationSettingId,
	[Name],
	[Required]
from ApplicationSetting with (nolock)
where ApplicationId = @ApplicationId