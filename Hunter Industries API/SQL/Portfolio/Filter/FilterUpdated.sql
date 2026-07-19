update PortfolioFilter set
	[Name] = @name,
	[Type] = @type,
	[Operator] = @operator,
	[Path] = @path,
	[Values] = @values
where PortfolioFilterId = @filterId