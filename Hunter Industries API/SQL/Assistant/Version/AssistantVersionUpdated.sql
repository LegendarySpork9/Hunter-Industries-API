update AssistantInformation set VersionId = (
	select
		VersionId
	from [Version] with (nolock)
	where [Value] = @Version
)
where [Name] = @AssistantName
and IDNumber = @IDNumber