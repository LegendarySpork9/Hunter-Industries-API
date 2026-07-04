select
(
	select
		count(*) as Items
	from PortfolioItem with (nolock)
	where IsDeleted = 0
) as PortfolioItems,
(
	select
		count(*) as Filters
	from PortfolioFilter with (nolock)
	where IsDeleted = 0
) as PortfolioFilters,
(
	select
		count(*) as AIUsed
	from PortfolioItem with (nolock)
	where IsDeleted = 0
	and LLMModelId is not null
) as PortfolioItemsAIUsed