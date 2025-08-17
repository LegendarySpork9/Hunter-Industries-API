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
	join Component with (nolock) on CI.ComponentID = Component.ComponentID
	join ComponentStatus CS with (nolock) on CI.ComponentStatusID = CS.ComponentStatusID
	join ServerInformation SI with (nolock) on CI.ServerInformationID = SI.ServerInformationID
	join Machine with (nolock) on SI.MachineID = Machine.MachineID
	join Game with (nolock) on SI.GameID = Game.GameID
	where Component.[Name] = @Component
)
select Component, [Status], HostName, Game, [Version], DateOccured from RankedComponentInformation
where rn = 1
order by 6 desc