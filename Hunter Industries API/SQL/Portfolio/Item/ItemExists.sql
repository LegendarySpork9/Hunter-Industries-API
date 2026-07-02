select
	PortfolioItemId
from PortfolioItem with (nolock)
where IsDeleted = 0