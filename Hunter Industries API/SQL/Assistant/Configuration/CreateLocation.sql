insert into [Location] (HostName, IPAddress)
output inserted.LocationID
values (@Hostname, @IPAddress)