select count(*) from Media with (nolock)
join [Application] with (nolock) on Media.ApplicationId = [Application].ApplicationId
where [Application].[Name] = @application