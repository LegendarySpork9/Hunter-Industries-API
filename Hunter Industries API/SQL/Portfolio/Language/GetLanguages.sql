select
	PortfolioItemId,
	[Name]
from PortfolioItemLanguage PIL with (nolock)
join [Language] L with (nolock) on PIL.LanguageId = L.LanguageId
join PortfolioItem [PI] with (nolock) on PIL.PortfolioItemId = [PI].PortfolioItemId