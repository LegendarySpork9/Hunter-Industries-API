if (
	select
		count(*)
	from Environment with (nolock)
	where [Name] = @name
) = 0 
begin
	
	insert into Environment ([Name])
	values (
		@name
	)

end