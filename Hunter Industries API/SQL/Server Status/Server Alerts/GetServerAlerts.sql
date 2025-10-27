select
	ServerAlertsID,
	US.[Value],
	Component.[Name],
	CS.[Value],
	SAS.[Value],
	DateOccured,
	HostName,
	Game.[Name],
	[Version]
from ServerAlert SA with (nolock)
join UserSetting US with (nolock) on SA.UserSettingsID = US.UserSettingsID
join Component with (nolock) on SA.ComponentID = Component.ComponentID
join ComponentStatus CS with (nolock) on SA.ComponentStatusID = CS.ComponentStatusID
join ServerAlertStatus SAS with (nolock) on SA.AlertStatusID = SAS.AlertStatusID
join ServerInformation SI with (nolock) on SA.ServerInformationID = SI.ServerInformationID
join Machine with (nolock) on SI.MachineID = Machine.MachineID
join Game with (nolock) on SI.GameID = Game.GameID
order by DateOccured desc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only