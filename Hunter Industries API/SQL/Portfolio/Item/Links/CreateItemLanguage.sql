insert into PortfolioItemLanguage(PortfolioItemId, LanguageId)
select
	@itemId,
	LanguageId
from [Language] with (nolock)
where [Language].[Name] = @language