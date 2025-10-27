select
	AI.[Name],
	AI.IDNumber,
	L.HostName,
	L.IPAddress
from AssistantInformation AI with (nolock)
join Location L with (nolock) on AI.LocationId = L.LocationId
where AI.[Name] = @AssistantName
and AI.IDNumber = @AssistantId