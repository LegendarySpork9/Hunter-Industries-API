update [Location] set HostName = @HostName, IPAddress = @IPAddress
from [Location] L
join AssistantInformation AI with (nolock) on L.LocationId = AI.LocationId
where [Name] = @AssistantName
and IDNumber = @IDNumber