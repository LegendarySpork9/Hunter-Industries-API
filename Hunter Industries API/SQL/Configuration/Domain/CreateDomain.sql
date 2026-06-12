insert into Domain (Host)
output inserted.DomainId
values (
	@domain
)