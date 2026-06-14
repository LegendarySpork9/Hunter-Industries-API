update Media set
	[Name] = @name,
	Size = @size,
	[Path] = @path,
	DateUpdated = GETUTCDATE()
where MediaId = @mediaId