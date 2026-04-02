update [ApplicationSetting] set
	[Name] = @Name,
	[Type] = @Type,
	[Required] = @Required
where ApplicationSettingId = @ApplicationSettingId