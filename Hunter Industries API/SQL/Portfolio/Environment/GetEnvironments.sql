select
	[PI].PortfolioItemId,
	E.[Name]
from PortfolioItemEnvironment PIE with (nolock)
join Environment E with (nolock) on PIE.EnvironmentId = E.EnvironmentId
join PortfolioItem [PI] with (nolock) on PIE.PortfolioItemId = [PI].PortfolioItemId