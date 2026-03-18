insert into UserSetting (UserId, ApplicationId, [Name], [Value])
output inserted.UserSettingId
select
	AU.UserId,
	A.ApplicationId,
	@Name,
	@Value
from APIUser AU with (nolock)
join [Application] A with (nolock) on A.[Name] = @Application
where AU.Username = @Username