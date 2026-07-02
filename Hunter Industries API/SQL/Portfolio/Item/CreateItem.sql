insert into PortfolioItem (TypeId, LLMModelId, [Name], Summary, [Description], IconURL, ReleaseNotes, GitHubLink, DemoLink, UnitTestCoverage, LLMUsageNotes, DateCreated, DateUpdated)
output inserted.PortfolioItemId
select
	PIT.PortfolioItemTypeId,
	LLMModel.LLMModelId,
	@name,
	@summary,
	@description,
	@icon,
	@releaseNotes,
	@gitHub,
	@demo,
	@unitTestCoverage,
	@llmUsageNotes,
	GETUTCDATE(),
	GETUTCDATE()
from PortfolioItemType PIT with (nolock)
left join LLMModel with (nolock) on LLMModel.[Name] = @model
join LLMCompany with (nolock) on LLMModel.LLMCompanyId = LLMCompany.LLMCompanyId
	and LLMCompany.[Name] = @company
where PIT.[Name] = @type