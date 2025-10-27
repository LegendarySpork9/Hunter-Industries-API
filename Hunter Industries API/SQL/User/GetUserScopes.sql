select
	[Value]
from UserScope with (nolock)
join APIUser with (nolock) on UserScope.UserID = APIUser.UserID
join Scope with (nolock) on UserScope.ScopeID = Scope.ScopeID
where UserScopeID is not null