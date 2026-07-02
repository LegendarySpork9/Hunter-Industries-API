select
	W.[Name] as Workflow,
	S.[Value] as [Status]
from Repository R with (nolock)
join WorkflowRun WR with (nolock) on R.RepositoryId = WR.RepositoryId
join Workflow W with (nolock) on WR.WorkflowId = W.WorkflowId
join [Status] S with (nolock) on WR.ConclusionId = S.StatusId
where R.[Name] = @repository
and RunNumber = (
	select
		max(WR2.RunNumber)
	from WorkflowRun WR2 with (nolock)
	where WR2.RepositoryId = WR.RepositoryId
	and WR2.WorkflowId = WR.WorkflowId
)