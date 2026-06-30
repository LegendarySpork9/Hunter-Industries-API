select
	MediaId,
	Extension,
	MimeType,
	Media.[Name] as [FileName],
	Size,
	[Path],
	Host,
	DateUploaded,
	DateUpdated,
	Media.IsDeleted
from Media with (nolock)
join MediaType MT with (nolock) on Media.MediaTypeId = MT.MediaTypeId
join Domain with (nolock) on Media.DomainId = Domain.DomainId