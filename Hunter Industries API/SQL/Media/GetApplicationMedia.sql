select
	MediaId,
	Extension,
	MimeType,
	Media.[Name] as [FileName],
	Size,
	[Path],
	Host,
	[Application].[Name] as [App],
	DateUploaded,
	DateUpdated,
	Media.IsDeleted
from Media with (nolock)
join MediaType MT with (nolock) on Media.MediaTypeId = MT.MediaTypeId
join Domain with (nolock) on Media.DomainId = Domain.DomainId
join [Application] with (nolock) on Media.ApplicationId = [Application].ApplicationId
where [Application].[Name] = @application