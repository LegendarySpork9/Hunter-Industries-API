select [Value] from UserScope with (nolock)
join APIUser on UserScope.UserID = APIUser.UserID
join Scope on UserScope.ScopeID = Scope.ScopeID
where UserScopeID is not null