update Media set
	IsDeleted = 1,
	DateUpdated = GETUTCDATE()
where MediaId = @mediaId