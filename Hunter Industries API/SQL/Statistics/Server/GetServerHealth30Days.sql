with Dates as (
    select datefromparts(year(getutcdate()), month(getutcdate()), 1) as DateOccured
    union all
    select dateadd(day, 1, DateOccured)
    from Dates
    where DateOccured < cast(getutcdate() as date)
)
select
    format(Dates.DateOccured, 'MMM dd') as [Date],
    cast(
        (min(isnull(OnlineEvents.EventCount, 0)) * SI.EventInterval)
        / (86400.0 - isnull(D.Duration, 0))
        * 100
    as decimal(5, 2)) as UptimePercentage
from ServerInformation SI with (nolock)
left join Downtime D with (nolock) ON SI.DowntimeId = D.DowntimeId
cross join Dates
cross apply (
    select distinct CI.ComponentId
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
) OnlineEvents on OnlineEvents.ComponentId = ServerComponents.ComponentId and OnlineEvents.EventDate = Dates.DateOccured
where ServerInformationId = @serverId
group by Dates.DateOccured, SI.EventInterval, D.Duration
order by Dates.DateOccured asc