update AssistantInformation set VersionId = (
	select
		VersionId
	from [Version] with (nolock)
	where [Value] = @version
)
where [Name] = @assistantName
and IDNumber = @idNumber