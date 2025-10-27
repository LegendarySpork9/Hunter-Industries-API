select
	[Value]
from UserScope with (nolock)
join APIUser with (nolock) on UserScope.UserId = APIUser.UserId
join Scope with (nolock) on UserScope.ScopeId = Scope.ScopeId
where UserScopeId is not null