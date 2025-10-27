insert into UserSetting (UserId, ApplicationId, [Name], [Value])
output inserted.UserSettingId
values (
	(
		select
			UserId
		from APIUser with (nolock)
		where Username = @Username
	),
	(
		select
			ApplicationId
		from [Application] with (nolock)
		where [Name] = @Application
	),
	@Name,
	@Value
)