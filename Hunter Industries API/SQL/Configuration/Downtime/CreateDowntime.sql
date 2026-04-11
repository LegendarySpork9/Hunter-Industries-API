insert into Downtime ([Time], [Duration])
output inserted.DowntimeId
values (
	@time,
	@duration
)