insert into [AssistantInformation] (LocationId, DeletionStatusId, VersionId, UserId, [Name], IDNumber)
values (
	@LocationId,
	2,
	(
		select
			max(VersionId)
		from [Version] with (nolock)
	),
	@UserId,
	@AssistantName,
	@IDNumber
)