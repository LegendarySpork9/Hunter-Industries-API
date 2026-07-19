insert into PortfolioFilter ([Name], [Type], [Operator], [Path], [Values])
output inserted.PortfolioFilterId
values (
	@name,
	@type,
	@operator,
	@path,
	@values
)