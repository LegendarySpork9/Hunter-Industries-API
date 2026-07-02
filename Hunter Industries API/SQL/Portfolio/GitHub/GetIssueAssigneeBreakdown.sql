select
	Username,
	count(*) as Issues
from Issue with (nolock)
join Repository with (nolock) on Issue.RepositoryId = Repository.RepositoryId
join [Status] with (nolock) on Issue.StatusId = [Status].StatusId
join [User] with (nolock) on Issue.AssigneeId = [User].UserId
where Repository.[Name] = @repository
and [Status].[Value] = 'Open'
group by Username