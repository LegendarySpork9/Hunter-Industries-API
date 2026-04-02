update [Location] set HostName = @hostName, IPAddress = @ipAddress
from [Location] L
join AssistantInformation AI with (nolock) on L.LocationId = AI.LocationId
where [Name] = @assistantName
and IDNumber = @idNumber