update AssistantInformation set DeletionStatusId = (
	select
		StatusId
	from [Deletion] with (nolock)
	where Value = @Deletion
)
where [Name] = @AssistantName
and IDNumber = @IDNumber