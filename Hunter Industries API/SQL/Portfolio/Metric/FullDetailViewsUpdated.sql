if (
	select
		count(*)
	from PortfolioItemMetric with (nolock)
	where PortfolioItemId = @itemId
) = 0 
begin
	
	insert into PortfolioItemMetric (PortfolioItemId, FullDetailViews)
	values (
		@itemId,
		1
	)

end
else
begin

	update PortfolioItemMetric set FullDetailViews = FullDetailViews + 1
	where PortfolioItemId = @itemId

end