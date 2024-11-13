select AI.Name, AI.IDNumber, D.Value from AssistantInformation AI with (nolock)
join Deletion D with (nolock) on AI.DeletionStatusID = D.StatusID
where AI.Name = @AssistantName
and AI.IDNumber = @AssistantID