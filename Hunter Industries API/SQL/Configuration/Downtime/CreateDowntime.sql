insert into Downtime ([Time])
output inserted.DowntimeId
values (
	@Time
)