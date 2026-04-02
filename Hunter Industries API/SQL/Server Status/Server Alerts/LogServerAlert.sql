insert into ServerAlert (ServerInformationId, UserSettingId, ComponentId, ComponentStatusId, AlertStatusId, DateOccured)
output inserted.ServerAlertId
select
	@serverId,
	US.UserSettingId,
	C.ComponentId,
	CS.ComponentStatusId,
	SAS.AlertStatusId,
	GETUTCDATE()
from UserSetting US with (nolock)
join [Application] A with (nolock) on A.ApplicationId = US.ApplicationId
	and A.[Name] = @application
join Component C with (nolock) on C.[Name] = @component
join ComponentStatus CS with (nolock) on CS.[Value] = @componentStatus
join ServerAlertStatus SAS with (nolock) on SAS.[Value] = @alertStatus
where US.[Name] = 'DiscordName'
and US.[Value] = @reporter