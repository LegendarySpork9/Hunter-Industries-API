insert into UserSetting (UserID, ApplicationID, [Name], [Value])
output inserted.UserSettingsID
values (
	(
		select
			UserID
		from APIUser with (nolock)
		where Username = @Username
	),
	(
		select
			ApplicationID
		from [Application] with (nolock)
		where [Name] = @Application
	),
	@Name,
	@Value
)