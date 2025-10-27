insert into [AssistantInformation] (LocationId, DeletionStatusId, VersionId, UserId, [Name], IDNumber)
values (
	@LocationId,
	2,
	(
		select top 1
			VersionId
		from [Version] with (nolock)
		order by VersionId desc
	),
	@UserId,
	@AssistantName,
	@IDNumber
)