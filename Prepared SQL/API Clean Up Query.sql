use [HunterIndustriesAPI]

declare @CleanUpResults table (
	[Table] varchar(255),
	Expected int,
	Actual int,
	[Match] int
)

declare @expected int
declare @actual int
declare @match int

set @expected = (
	select count(*) from [User] with (nolock)
	where UserID not in
	(
		select UserID from AssistantInformation with (nolock)
	)
)

delete from [User]
where UserID not in 
(
	select UserID from AssistantInformation with (nolock)
)

set @actual = (SELECT @@ROWCOUNT)

set @match = case
when @actual = @expected then 1
else 0
end

insert into @CleanUpResults ([Table], Expected, Actual, [Match])
values ('User', @expected, @actual, @match)

set @expected = (
	select count(*) from [Location] with (nolock)
	where LocationID not in
	(
		select LocationID from AssistantInformation with (nolock)
	)
)

delete from [Location]
where LocationID not in 
(
	select LocationID from AssistantInformation with (nolock)
)

set @actual = (SELECT @@ROWCOUNT)

set @match = case
when @actual = @expected then 1
else 0
end

insert into @CleanUpResults ([Table], Expected, Actual, [Match])
values ('Location', @expected, @actual, @match)

select * from @CleanUpResults