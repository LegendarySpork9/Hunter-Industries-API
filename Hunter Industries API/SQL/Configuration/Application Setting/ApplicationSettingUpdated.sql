update [ApplicationSetting] set
	[Name] = @name,
	[Type] = @type,
	[Required] = @required
where ApplicationSettingId = @applicationSettingId