select
	ServerAlertId,
	US.[Value],
	Component.[Name],
	CS.[Value],
	SAS.[Value],
	DateOccured,
	HostName,
	Game.[Name],
	[Version]
from ServerAlert SA with (nolock)
join UserSetting US with (nolock) on SA.UserSettingId = US.UserSettingId
join Component with (nolock) on SA.ComponentId = Component.ComponentId
join ComponentStatus CS with (nolock) on SA.ComponentStatusId = CS.ComponentStatusId
join ServerAlertStatus SAS with (nolock) on SA.AlertStatusId = SAS.AlertStatusId
join ServerInformation SI with (nolock) on SA.ServerInformationId = SI.ServerInformationId
join Machine with (nolock) on SI.MachineId = Machine.MachineId
join Game with (nolock) on SI.GameId = Game.GameId
order by DateOccured desc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only