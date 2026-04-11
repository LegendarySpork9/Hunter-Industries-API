insert into UserScope (UserId, ScopeId)
output inserted.UserScopeId
values (
	@userId,
	(
		select
			ScopeId
		from Scope with (nolock)
		where [Value] = @scope
	)
)