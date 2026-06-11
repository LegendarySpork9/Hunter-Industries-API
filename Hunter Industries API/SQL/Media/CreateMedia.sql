insert into Media (MediaTypeId, DomainId, ApplicationId, [Name], Size, [Path], DateUploaded, DateUpdated)
output inserted.MediaId
select
	MediaType.MediaTypeId,
	Domain.DomainId,
	[Application].ApplicationId,
	@name,
	@size,
	@path,
	GETUTCDATE(),
	GETUTCDATE()
from (select 1 as Dummy) D
join MediaType with (nolock) on MediaType.Extension = @extension
	and MediaType.MimeType = @mimeType
join Domain with (nolock) on Domain.Host = @domain
	and Domain.IsDeleted = 0
join [Application] with (nolock) on [Application].[Name] = @application
	and [Application].IsDeleted = 0