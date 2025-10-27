insert into ServerAlert (ServerInformationId, UserSettingId, ComponentId, ComponentStatusId, AlertStatusId, DateOccured)
output inserted.ServerAlertsId
values (
	@ServerId,
	(
		select
			UserSettingId
		from UserSetting with (nolock)
		where ApplicationId = (
			select
				ApplicationId
			from [Application] with (nolock)
			where [Name] = 'Server Status Site'
		)
		and [Name] = 'DiscordName'
		and [Value] = @Reporter
	),
	(
		select
			ComponentId
		from Component with (nolock)
		where [Name] = @Component
	),
	(
		select
			ComponentStatusId
		from ComponentStatus with (nolock)
		where [Value] = @ComponentStatus
	),
	(
		select
			AlertStatusId
		from ServerAlertStatus with (nolock)
		where [Value] = @AlertStatus
	),
	GETDATE()
)