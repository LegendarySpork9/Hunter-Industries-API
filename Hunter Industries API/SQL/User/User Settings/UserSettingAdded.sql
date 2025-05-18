insert into UserSettings (UserID, ApplicationID, [Name], [Value])
output inserted.UserSettingsID
values ((select UserID from APIUser with (nolock) where Username = @Username), (select ApplicationID from [Application] where [Name] = @Application), @Name, @Value)