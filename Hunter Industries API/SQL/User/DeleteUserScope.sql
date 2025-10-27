delete from UserScope
where UserId = @userId
and ScopeId = (
	select
		ScopeId
	from Scope with (nolock)
	where [Value] = @scope
)