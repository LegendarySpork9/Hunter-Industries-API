select
	AI.[Name],
	AI.IDNumber,
	V.[Value] as [Version]
from AssistantInformation AI with (nolock)
join [Version] V with (nolock) on AI.VersionId = V.VersionId
where AI.[Name] = @assistantName
and AI.IDNumber = @assistantId