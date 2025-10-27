insert into UserScope (UserId, ScopeId)
output inserted.UserScopeId
values (
	@UserId,
	(
		select
			ScopeId
		from Scope with (nolock)
		where [Value] = @Scope
	)
)