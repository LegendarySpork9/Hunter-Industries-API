if (
	select
		count(*)
	from Framework with (nolock)
	where [Name] = @name
) = 0 
begin
	
	insert into Framework ([Name])
	values (
		@name
	)

end