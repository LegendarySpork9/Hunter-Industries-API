update [Location] set HostName = @HostName, IPAddress = @IPAddress
where LocationID = (
	select LocationID from AssistantInformation with (nolock)
	where Name = @AssistantName
	and IDNumber = @IDNumber
)