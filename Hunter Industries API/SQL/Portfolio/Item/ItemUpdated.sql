update PortfolioItem set
	[Name] = @name,
	TypeId = PIT.PortfolioItemTypeId,
	IconURL = @icon,
	Summary = @summary,
	[Description] = @description,
	DemoLink = @demo,
	ReleaseNotes = @releaseNotes,
	UnitTestCoverage = @unitTestCoverage,
	GitHubLink = @gitHub,
	LLMModelId = LLMModel.LLMModelId,
	LLMUsageNotes = @llmUsageNotes,
	DateUpdated = GETUTCDATE()
from PortfolioItem [PI] with (nolock)
join PortfolioItemType PIT with (nolock) on [PI].TypeId = @type
left join LLMModel with (nolock) on LLMModel.[Name] = @model
join LLMCompany with (nolock) on LLMModel.LLMCompanyId = LLMCompany.LLMCompanyId
	and LLMCompany.[Name] = @company
where PortfolioItemId = @itemId