insert into PortfolioFilter ([Name], [Values])
output inserted.PortfolioFilterId
values (
	@name,
	@values
)