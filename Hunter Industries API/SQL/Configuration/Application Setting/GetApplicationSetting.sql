select
	ApplicationSettingId,
	[Name],
	[Type],
	[Required],
	IsDeleted
from ApplicationSetting with (nolock)