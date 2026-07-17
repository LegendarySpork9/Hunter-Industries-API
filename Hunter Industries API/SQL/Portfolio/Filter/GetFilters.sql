select
	PortfolioFilterId,
	[Name],
	[Type],
	[Operator],
	[Path],
	[Values],
	IsDeleted
from PortfolioFilter with (nolock)