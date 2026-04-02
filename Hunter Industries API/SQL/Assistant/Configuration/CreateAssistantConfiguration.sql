insert into [AssistantInformation] (LocationId, DeletionStatusId, VersionId, UserId, [Name], IDNumber)
values (
	@locationId,
	2,
	(
		select
			max(VersionId)
		from [Version] with (nolock)
	),
	@userId,
	@assistantName,
	@idNumber
)