select top 5
	[Name],
	SummaryViews,
	FullDetailViews,
	SummaryViews + FullDetailViews as [TotalViews]
from PortfolioItem [PI] with (nolock)
join PortfolioItemMetric PIM with (nolock) on [PI].PortfolioItemId = PIM.PortfolioItemId
order by [TotalViews] desc