if (
	select count(*)
	from MediaType with (nolock)
	where Extension = @extension
	and MimeType = @mimeType
) = 0 
begin
	
	insert into MediaType (Extension, MimeType)
	values (
		@extension,
		@mimeType
	)

end