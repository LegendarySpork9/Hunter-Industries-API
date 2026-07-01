if (
	select
		count(*)
	from [Language] with (nolock)
	where [Name] = @name
) = 0 
begin
	
	insert into [Language] ([Name])
	values (
		@name
	)

end