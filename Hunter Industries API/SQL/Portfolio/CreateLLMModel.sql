if (
	select
		count(*)
	from LLMModel with (nolock)
	join LLMCompany with (nolock) on LLMModel.LLMCompanyId = LLMCompany.LLMCompanyId
	where LLMCompany.[Name] = @company
	and LLMModel.[Name] = @model
) = 0 
begin
	
	insert into LLMModel (LLMCompanyId, [Name])
	select
		LLMCompany.LLMCompanyId,
		@model
	from LLMCompany with (nolock)
	where LLMCompany.[Name] = @company

end