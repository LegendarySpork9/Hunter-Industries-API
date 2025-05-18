select UserSettingsID, UserSettings.[Name], [Value] from UserSettings with (nolock)
where UserSettingsID = @Id