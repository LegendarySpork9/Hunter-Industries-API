select
(
	select
		count(*) as Issues
	from Issue with (nolock)
	join Repository with (nolock) on Issue.RepositoryId = Repository.RepositoryId
	join [Status] with (nolock) on Issue.StatusId = [Status].StatusId
	where Repository.[Name] = @repository
	and [Status].[Value] = 'Open'
) as TotalIssues,
(
	select
		count(*) as Issues
	from Issue with (nolock)
	join Repository with (nolock) on Issue.RepositoryId = Repository.RepositoryId
	join [Status] with (nolock) on Issue.StatusId = [Status].StatusId
	join [Type] with (nolock) on Issue.TypeId = [Type].TypeId
	where Repository.[Name] = @repository
	and [Status].[Value] = 'Open'
	and [Type].[Value] = 'bug'
) as Bugs,
(
	select
		count(*) as Issues
	from Issue with (nolock)
	join Repository with (nolock) on Issue.RepositoryId = Repository.RepositoryId
	join [Status] with (nolock) on Issue.StatusId = [Status].StatusId
	join [Type] with (nolock) on Issue.TypeId = [Type].TypeId
	where Repository.[Name] = @repository
	and [Status].[Value] = 'Open'
	and [Type].[Value] = 'new feature'
) as NewFeatures