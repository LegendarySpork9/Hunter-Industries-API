insert into [AssistantInformation] (LocationID, DeletionStatusID, VersionID, UserID, [Name], IDNumber)
values (
	@LocationID,
	2,
	(
		select top 1
			VersionID
		from [Version] with (nolock)
		order by VersionID desc
	),
	@UserID,
	@AssistantName,
	@IDNumber
)