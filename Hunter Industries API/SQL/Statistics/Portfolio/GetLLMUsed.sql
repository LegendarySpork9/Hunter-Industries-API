select
	LLMCompany.[Name] as Company,
	LLMModel.[Name] as Model,
	count(*) as Uses
from LLMCompany with (nolock)
join LLMModel with (nolock) on LLMCompany.LLMCompanyId = LLMModel.LLMCompanyId
join PortfolioItem [PI] with (nolock) on LLMModel.LLMModelId = [PI].LLMModelId
group by
	LLMCompany.[Name],
	LLMModel.[Name]
order by Uses desc