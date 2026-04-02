insert into [ApplicationSetting] (ApplicationId, [Name], [Type], [Required])
output inserted.ApplicationSettingId
values (
	@applicationId,
	@name,
	@type,
	@required
)