insert into UserSetting (UserId, ApplicationId, [Name], [Value])
output inserted.UserSettingId
select
	AU.UserId,
	A.ApplicationId,
	@name,
	@value
from APIUser AU with (nolock)
join [Application] A with (nolock) on A.[Name] = @application
where AU.Username = @username