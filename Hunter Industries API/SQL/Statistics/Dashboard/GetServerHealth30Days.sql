SELECT
    SI.ServerInformationId,
    SI.[Name],
    EventDays.EventDate AS Date,
    CAST(
        (MIN(ISNULL(OnlineEvents.EventCount, 0)) * SI.EventInterval)
        / (86400.0 - ISNULL(D.Duration, 0))
        * 100
    AS DECIMAL(5, 2)) AS UptimePercentage
FROM ServerInformation SI with (nolock)
LEFT JOIN Downtime D with (nolock) ON SI.DowntimeId = D.DowntimeId
CROSS APPLY (
    SELECT
        CAST(CI.DateOccured AS DATE) AS EventDate,
        COUNT(*) / COUNT(DISTINCT CI.ComponentId) AS EventCount
    FROM ComponentInformation CI with (nolock)
    WHERE CI.ServerInformationId = SI.ServerInformationId
    AND CI.DateOccured >= DATEADD(DAY, -30, GETUTCDATE())
    GROUP BY CAST(CI.DateOccured AS DATE)
) EventDays
CROSS APPLY (
    SELECT DISTINCT
      CI.ComponentId
    FROM ComponentInformation CI with (nolock)
    WHERE CI.ServerInformationId = SI.ServerInformationId
) ServerComponents
LEFT JOIN (
    SELECT
      CI.ComponentId,
      CAST(CI.DateOccured AS DATE) AS EventDate,
      COUNT(*) AS EventCount
    FROM ComponentInformation CI with (nolock)
    INNER JOIN ComponentStatus CS with (nolock) ON CI.ComponentStatusId = CS.ComponentStatusId
    WHERE CS.[Value] = 'Online'
    GROUP BY CI.ComponentId, CAST(CI.DateOccured AS DATE)
) OnlineEvents ON OnlineEvents.ComponentId = ServerComponents.ComponentId
    AND OnlineEvents.EventDate = EventDays.EventDate
WHERE SI.IsActive = 1
GROUP BY SI.ServerInformationId, SI.[Name], EventDays.EventDate, EventDays.EventCount
ORDER BY SI.ServerInformationId, EventDays.EventDate asc