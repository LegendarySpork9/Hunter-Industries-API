select count(*) from ServerAlert SA with (nolock)
join ServerInformation SI with (nolock) on SA.ServerInformationId = SI.ServerInformationId
where ServerAlertId is not null