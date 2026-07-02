insert into PortfolioItemFramework(PortfolioItemId, FrameworkId)
select
	@itemId,
	FrameworkId
from Framework with (nolock)
where Framework.[Name] = @framework