insert into [ApplicationSetting] (ApplicationId, [Name], [Type], [Required])
output inserted.ApplicationSettingId
values (
	@ApplicationId,
	@Name,
	@Type,
	@Required
)