update Media set
	[Name] = @name,
	Size = @size,
	[Path] = @path
where MediaId = @mediaId