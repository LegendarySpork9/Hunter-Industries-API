update ServerAlert set AlertStatusId = (
	select
		AlertStatusId
	from ServerAlertStatus with (nolock)
	where [Value] = @AlertStatus
)
where ServerAlertId = @AlertId