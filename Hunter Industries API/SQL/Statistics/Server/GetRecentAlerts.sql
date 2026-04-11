select top 5
	ServerAlertId,
	US.[Value] as Reporter,
	Component.[Name] as Component,
	CS.[Value] as ComponentStatus,
	SAS.[Value] as AlertStatus,
	DateOccured
from ServerAlert SA with (nolock)
join UserSetting US with (nolock) on SA.UserSettingId = US.UserSettingId
join Component with (nolock) on SA.ComponentId = Component.ComponentId
join ComponentStatus CS with (nolock) on SA.ComponentStatusId = CS.ComponentStatusId
join ServerAlertStatus SAS with (nolock) on SA.AlertStatusId = SAS.AlertStatusId
where ServerInformationId = @serverId
order by DateOccured desc