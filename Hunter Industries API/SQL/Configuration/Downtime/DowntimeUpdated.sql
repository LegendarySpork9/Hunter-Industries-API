update Downtime set
	[Time] = @time,
	Duration = @duration
where DowntimeId = @downtimeId