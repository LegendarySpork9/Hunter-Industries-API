delete from UserScope
where UserID = @userId
and ScopeID = (
	select ScopeID from Scope with (nolock)
	where [Value] = @scope
)