with Dates as (
    select datefromparts(year(getutcdate()), month(getutcdate()), 1) as DateOccured
    union all
    select dateadd(day, 1, DateOccured)
    from Dates
    where DateOccured < cast(getutcdate() as date)
)
select
    format(Dates.DateOccured, 'MMM dd') as [Day],
    isnull(SuccessfulCalls.CallCount, 0) as SuccessfulCalls,
    isnull(UnsuccessfulCalls.CallCount, 0) as UnsuccessfulCalls
from Dates
left join (
    select
        format(DateOccured, 'MMM dd') as [Day],
        count (*) as CallCount
    from AuditHistory with (nolock)
    join StatusCode with (nolock) on AuditHistory.StatusId = StatusCode.StatusId
    where DateOccured >= datefromparts(year(getutcdate()), month(getutcdate()), 1)
    and DateOccured < dateadd(day, 1, cast(getutcdate() as date))
    and [Value] like '20%'
    group by format(DateOccured, 'MMM dd')
) as SuccessfulCalls on format(Dates.DateOccured, 'MMM dd') = SuccessfulCalls.[Day]
left join (
    select
        format(DateOccured, 'MMM dd') as [Day],
        count (*) as CallCount
    from AuditHistory with (nolock)
    join StatusCode with (nolock) on AuditHistory.StatusId = StatusCode.StatusId
    where DateOccured >= datefromparts(year(getutcdate()), month(getutcdate()), 1)
    and DateOccured < dateadd(day, 1, cast(getutcdate() as date))
    and [Value] not like '20%'
    group by format(DateOccured, 'MMM dd')
) as UnsuccessfulCalls on format(Dates.DateOccured, 'MMM dd') = UnsuccessfulCalls.[Day]
order by Dates.DateOccured asc
option (maxrecursion 31)