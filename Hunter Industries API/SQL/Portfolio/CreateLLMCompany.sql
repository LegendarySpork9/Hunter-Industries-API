if (
	select
		count(*)
	from LLMCompany with (nolock)
	where [Name] = @name
) = 0 
begin
	
	insert into LLMCompany ([Name])
	values (
		@name
	)

end