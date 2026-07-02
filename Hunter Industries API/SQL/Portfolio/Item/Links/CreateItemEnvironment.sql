insert into PortfolioItemEnvironment (PortfolioItemId, EnvironmentId)
select
	@itemId,
	EnvironmentId
from Environment with (nolock)
where Environment.[Name] = @environment