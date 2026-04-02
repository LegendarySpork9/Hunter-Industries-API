select
	[Application].ApplicationId,
	[Application].[Name] as [Application],
	Authorisation.PhraseId,
	Phrase,
	ApplicationSettingId,
	[AS].[Name] as Setting,
	[Type],
	[Required],
	[AS].IsDeleted as SettingDeleted,
	[Application].IsDeleted as ApplicationDeleted
from [Application] with (nolock)
join Authorisation with (nolock) on [Application].PhraseId = Authorisation.PhraseId
left join ApplicationSetting [AS] with (nolock) on [Application].ApplicationId = [AS].ApplicationId