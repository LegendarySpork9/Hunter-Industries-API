select
	ApplicationSettingId,
	[Name],
	[Required],
	IsDeleted
from ApplicationSetting with (nolock)
where ApplicationId = @ApplicationId