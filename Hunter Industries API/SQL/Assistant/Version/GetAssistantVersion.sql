select
	AI.[Name],
	AI.IDNumber,
	V.[Value]
from AssistantInformation AI with (nolock)
join [Version] V with (nolock) on AI.VersionID = V.VersionID
where AI.[Name] = @AssistantName
and AI.IDNumber = @AssistantID