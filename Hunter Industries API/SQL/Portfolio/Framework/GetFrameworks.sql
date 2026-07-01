select
	PortfolioItemId,
	[Name]
from PortfolioItemFramework PIF with (nolock)
join Framework F with (nolock) on PIF.FrameworkId = F.FrameworkId
join PortfolioItem [PI] with (nolock) on PIF.PortfolioItemId = [PI].PortfolioItemId