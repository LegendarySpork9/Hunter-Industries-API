insert into [User] (Name)
output inserted.UserID
values (@Name)