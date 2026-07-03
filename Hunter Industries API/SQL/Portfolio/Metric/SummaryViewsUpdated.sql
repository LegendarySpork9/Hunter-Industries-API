if (
	select
		count(*)
	from PortfolioItemMetric with (nolock)
	where PortfolioItemId = @itemId
) = 0 
begin
	
	insert into PortfolioItemMetric (PortfolioItemId, SummaryViews)
	values (
		@itemId,
		1
	)

end
else
begin

	update PortfolioItemMetric set SummaryViews = SummaryViews + 1
	where PortfolioItemId = @itemId

end