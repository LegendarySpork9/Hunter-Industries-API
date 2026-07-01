if (
	select
		count(*)
	from PortfolioItemType with (nolock)
	where [Name] = @name
) = 0 
begin
	
	insert into PortfolioItemType ([Name])
	values (
		@name
	)

end