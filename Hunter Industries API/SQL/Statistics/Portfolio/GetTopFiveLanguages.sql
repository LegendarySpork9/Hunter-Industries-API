select top 5
	[Name],
	count(*) as Uses
from [Language] L with (nolock)
join PortfolioItemLanguage PIL with (nolock) on L.LanguageId = PIL.LanguageId
group by [Name]
order by Uses desc