select
	[PI].PortfolioItemId,
	[PI].[Name] as [Name],
	PIT.[Name] as [Type],
	IconURL,
	Summary,
	[Description],
	DemoLink,
	ReleaseNotes,
	UnitTestCoverage,
	LLMCompany.[Name] as LLMCompany,
	LLMModel.[Name] as LLMModel,
	LLMUsageNotes,
	DateCreated,
	DateUpdated,
	IsDeleted
from PortfolioItem [PI] with (nolock)
join PortfolioItemType PIT with (nolock) on [PI].TypeId = PIT.PortfolioItemTypeId
left join LLMModel with (nolock) on [PI].LLMModelId = LLMModel.LLMModelId
join LLMCompany with (nolock) on LLMModel.LLMCompanyId = LLMCompany.LLMCompanyId