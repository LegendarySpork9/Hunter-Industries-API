select
	AI.[Name],
	AI.IDNumber,
	D.[Value] as DeletionStatus
from AssistantInformation AI with (nolock)
join Deletion D with (nolock) on AI.DeletionStatusId = D.StatusId
where AI.[Name] = @assistantName
and AI.IDNumber = @assistantId