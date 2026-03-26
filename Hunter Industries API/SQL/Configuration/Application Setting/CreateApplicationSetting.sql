insert into [ApplicationSetting] (ApplicationId, [Name], [Required])
output inserted.ApplicationSettingId
values (
	@ApplicationId,
	@Name,
	@Required
)