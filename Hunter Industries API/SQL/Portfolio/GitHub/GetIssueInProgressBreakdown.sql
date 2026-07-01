select
	Number as Id,
	Username as Assignee,
	Title,
	[Type].[Value] as [Type]
from Issue with (nolock)
join Repository with (nolock) on Issue.RepositoryId = Repository.RepositoryId
join [Status] with (nolock) on Issue.StatusId = [Status].StatusId
join [Type] with (nolock) on Issue.TypeId = [Type].TypeId
join [User] with (nolock) on Issue.AssigneeId = [User].UserId
where Repository.[Name] = @repository
and [Status].[Value] = 'Open'
and Username != 'Unassigned'