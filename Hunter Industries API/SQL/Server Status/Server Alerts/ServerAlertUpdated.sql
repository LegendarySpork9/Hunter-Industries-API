update ServerAlert set AlertStatusId = (
	select
		AlertStatusId
	from ServerAlertStatus with (nolock)
	where [Value] = @alertStatus
)
where ServerAlertId = @alertId