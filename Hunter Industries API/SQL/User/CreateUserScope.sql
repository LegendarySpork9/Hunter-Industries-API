insert into UserScope (UserID, ScopeID)
output inserted.UserScopeID
values (
	@UserID,
	(
		select
			ScopeID
		from Scope with (nolock)
		where [Value] = @Scope
	)
)