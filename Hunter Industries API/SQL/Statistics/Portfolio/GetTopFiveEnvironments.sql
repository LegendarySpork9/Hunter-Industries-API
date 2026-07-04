select top 5
	[Name],
	count(*) as Uses
from Environment E with (nolock)
join PortfolioItemEnvironment PIE with (nolock) on E.EnvironmentId = PIE.EnvironmentId
group by [Name]
order by Uses desc