insert into ServerAlert (ServerInformationId, UserSettingId, ComponentId, ComponentStatusId, AlertStatusId, DateOccured)
output inserted.ServerAlertId
select
	@ServerId,
	US.UserSettingId,
	C.ComponentId,
	CS.ComponentStatusId,
	SAS.AlertStatusId,
	GETUTCDATE()
from UserSetting US with (nolock)
join [Application] A with (nolock) on A.ApplicationId = US.ApplicationId
	and A.[Name] = @Application
join Component C with (nolock) on C.[Name] = @Component
join ComponentStatus CS with (nolock) on CS.[Value] = @ComponentStatus
join ServerAlertStatus SAS with (nolock) on SAS.[Value] = @AlertStatus
where US.[Name] = 'DiscordName'
and US.[Value] = @Reporter