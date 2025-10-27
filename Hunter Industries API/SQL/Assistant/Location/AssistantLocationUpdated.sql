update [Location] set HostName = @HostName, IPAddress = @IPAddress
where LocationId = (
	select
		LocationId
	from AssistantInformation with (nolock)
	where [Name] = @AssistantName
	and IDNumber = @IDNumber
)