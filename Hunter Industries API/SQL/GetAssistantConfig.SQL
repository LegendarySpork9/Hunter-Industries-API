﻿select AI.Name, IDNumber, U.Name, L.HostName, D.Value, V.Value from AssistantInformation AI with (nolock)
join [Location] L with (nolock) on AI.LocationID = L.LocationID
join Deletion D with (nolock) on AI.DeletionStatusID = D.StatusID
join [Version] V with (nolock) on AI.VersionID = V.VersionID
join [User] U with (nolock) on AI.UserID = U.UserID
where AI.Name is not null