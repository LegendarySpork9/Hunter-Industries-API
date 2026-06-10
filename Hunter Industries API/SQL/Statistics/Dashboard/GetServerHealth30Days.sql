select
    SI.ServerInformationId,
    SI.[Name],
    EventDays.EventDate as [Date],
    cast(
        (min(isnull(OnlineEvents.EventCount, 0)) * SI.EventInterval)
        / (86400.0 - isnull(D.Duration, 0))
        * 100
    as decimal(5, 2)) as UptimePercentage
from ServerInformation SI with (nolock)
left join Downtime D with (nolock) ON SI.DowntimeId = D.DowntimeId
cross apply (
    select
        cast(CI.DateOccured as date) as EventDate,
        count(*) / count(distinct CI.ComponentId) as EventCount
    from ComponentInformation CI with (nolock)
    where CI.ServerInformationId = SI.ServerInformationId
    and CI.DateOccured >= datefromparts(year(getutcdate()), month(getutcdate()), 1)
    and CI.DateOccured < dateadd(day, 1, eomonth(getutcdate()))
    group by cast(CI.DateOccured as date)
) EventDays
cross apply (
    select distinct
      CI.ComponentId
    from ComponentInformation CI with (nolock)
    where CI.ServerInformationId = SI.ServerInformationId
) ServerComponents
left join (
    select
      CI.ComponentId,
      cast(CI.DateOccured as date) as EventDate,
      count(*) as EventCount
    from ComponentInformation CI with (nolock)
    inner join ComponentStatus CS with (nolock) ON CI.ComponentStatusId = CS.ComponentStatusId
    where CS.[Value] = 'Online'
    group by CI.ComponentId, cast(CI.DateOccured as date)
) OnlineEvents on OnlineEvents.ComponentId = ServerComponents.ComponentId and OnlineEvents.EventDate = EventDays.EventDate
where SI.IsActive = 1
group by SI.ServerInformationId, SI.[Name], EventDays.EventDate, SI.EventInterval, D.Duration, EventDays.EventCount
order by SI.ServerInformationId, EventDays.EventDate asc