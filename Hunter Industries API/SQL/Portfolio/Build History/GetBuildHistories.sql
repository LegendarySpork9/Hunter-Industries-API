select
	PortfolioItemId,
	[Version],
	ReleaseDate
from PortfolioItemBuildHistory PIBH with (nolock)
join PortfolioItem [PI] with (nolock) on PIBH.PortfolioItemId = [PI].PortfolioItemId