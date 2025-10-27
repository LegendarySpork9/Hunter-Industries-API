update AssistantInformation set VersionID = (
	select
		VersionID
	from [Version] with (nolock)
	where [Value] = @Version
)
where [Name] = @AssistantName
and IDNumber = @IDNumber