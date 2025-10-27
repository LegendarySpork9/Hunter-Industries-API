select
	AI.[Name],
	IDNumber,
	U.[Name],
	L.HostName,
	D.[Value],
	V.[Value]
from AssistantInformation AI with (nolock)
join [Location] L with (nolock) on AI.LocationId = L.LocationId
join Deletion D with (nolock) on AI.DeletionStatusId = D.StatusId
join [Version] V with (nolock) on AI.VersionId = V.VersionId
join [User] U with (nolock) on AI.UserId = U.UserId
where AI.[Name] is not null