update AssistantInformation set DeletionStatusID = (select StatusID from [Deletion] with (nolock) where Value = @Deletion)
where Name = @AssistantName
and IDNumber = @IDNumber