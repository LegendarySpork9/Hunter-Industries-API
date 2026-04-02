update AssistantInformation set DeletionStatusId = (
	select
		StatusId
	from [Deletion] with (nolock)
	where Value = @deletion
)
where [Name] = @assistantName
and IDNumber = @idNumber