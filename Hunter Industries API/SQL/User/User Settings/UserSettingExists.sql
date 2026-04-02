select
	UserSettingId
from UserSetting with (nolock)
where UserId = (
	select
		UserId
	from APIUser with (nolock)
	where Username = @username
)
and ApplicationId = (
	select
		ApplicationId
	from [Application] with (nolock)
	where [Name] = @application
)
and [Name] = @name