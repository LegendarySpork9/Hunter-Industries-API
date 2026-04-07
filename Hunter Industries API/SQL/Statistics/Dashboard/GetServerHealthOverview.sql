select
	SI.ServerInformationId,
	isnull(EventCount, 0) as EventCount,
	isnull(AlertCount, 0) as AlertCount
from ServerInformation SI with (nolock)
left join (
	select
		ServerInformationId,
		count(*) as EventCount
	from ComponentInformation with (nolock)
	where DateOccured >= dateadd(day, -30, getutcdate())
	group by ServerInformationId
) [Events] on SI.ServerInformationId = [Events].ServerInformationId
left join (
	select
		ServerInformationId,
		count(*) as AlertCount
	from ServerAlert with (nolock)
	where DateOccured >= dateadd(day, -30, getutcdate())
	group by ServerInformationId
) Alerts on SI.ServerInformationId = Alerts.ServerInformationId
where IsActive = 1
order by SI.ServerInformationId asc