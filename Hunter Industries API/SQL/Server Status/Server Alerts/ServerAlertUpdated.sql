update ServerAlerts set AlertStatusID = (
	select
		AlertStatusID
	from ServerAlertStatus with (nolock)
	where [Value] = @AlertStatus
)
where ServerAlertsID = @AlertID