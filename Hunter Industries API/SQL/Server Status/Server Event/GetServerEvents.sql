with RankedComponentInformation as (
	select
		Component.[Name] as Component,
		CS.[Value] as [Status],
		HostName,
		Game.[Name] as Game,
		[Version],
		DateOccured,
		ROW_NUMBER() over (
			partition by Component.[Name], HostName, Game.[Name], [Version]
			order by DateOccured desc
		) as rn
	from ComponentInformation CI with (nolock)
	join Component with (nolock) on CI.ComponentId = Component.ComponentId
	join ComponentStatus CS with (nolock) on CI.ComponentStatusId = CS.ComponentStatusId
	join ServerInformation SI with (nolock) on CI.ServerInformationId = SI.ServerInformationId
	join Machine with (nolock) on SI.MachineId = Machine.MachineId
	join Game with (nolock) on SI.GameId = Game.GameId
	where Component.[Name] = @Component
)
select
	Component,
	[Status],
	HostName,
	Game,
	[Version],
	DateOccured
from RankedComponentInformation with (nolock)
where rn = 1
order by 6 desc