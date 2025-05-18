select [Application].[Name], UserSettingsID, UserSettings.[Name], [Value] from UserSettings with (nolock)
join [Application] with (nolock) on UserSettings.ApplicationID = [Application].ApplicationID
where UserSettingsID is not null