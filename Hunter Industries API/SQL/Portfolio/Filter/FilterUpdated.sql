update PortfolioFilter set
	[Name] = @name,
	[Values] = @values
where PortfolioFilterId = @filterId