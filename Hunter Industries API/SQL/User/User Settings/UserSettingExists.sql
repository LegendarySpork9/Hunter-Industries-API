select
	UserSettingId
from UserSetting with (nolock)
where UserId = (
	select
		UserId
	from APIUser with (nolock)
	where Username = @Username
)
and ApplicationId = (
	select
		ApplicationId
	from [Application] with (nolock)
	where [Name] = @Application
)
and [Name] = @Name