select
	UserSettingsID
from UserSettings with (nolock)
where UserID = (
	select
		UserID
	from APIUser with (nolock)
	where Username = @Username
)
and ApplicationID = (
	select
		ApplicationID
	from [Application] with (nolock)
	where [Name] = @Application
)
and [Name] = @Name