// Copyright © - unpublished - Toby Hunter
namespace HunterIndustriesAPI.Models
{
    public static class DatabaseModel
    {
        // Creates the variables and stores the commands the calls will execute.
        public static string? ConnectionString = null;
        public static string[] AssistantQueries = { @"select AI.Name, IDNumber, U.Name, L.HostName, D.Value, V.Value from AssistantInformation AI
join [Location] L on AI.LocationID = L.LocationID
join Deletion D on AI.DeletionStatusID = D.StatusID
join [Version] V on AI.VersionID = V.VersionID
join [User] U on AI.UserID = U.UserID", // 0
            @"update AssistantInformation set VersionID = (select VersionID from [Version] where Value = @Version)
where Name = @AssistantName
and IDNumber = @IDNumber", // 1
            @"update [Location] set HostName = @HostName, IPAddress = @IPAddress
where LocationID = (
	select LocationID from AssistantInformation
	where Name = @AssistantName
	and IDNumber = @IDNumber
)", // 2
            "" };
        public static string[] BookReaderQueries = { };
        public static string[] AssistantControlPanelQueries = { @"insert into [Location] (HostName, IPAddress)
values (@Hostname, @IPAddress)", // 0
            @"insert into [Version] (Value)
values (@Version)", // 1
            @"insert into [User] (Name)
values (@Name)", // 2
            @"insert into [AssistantInformation] (LocationID, DeletionStatusID, VersionID, UserID, Name, IDNumber)
values ((select top 1 LocationID from [Location] order by LocationID desc), 2, (select top 1 VersionID from [Version] order by VersionID desc), (select top 1 UserID from [User] order by UserID desc), @AssistantName, @IDNumber)", // 3
            @"update AssistantInformation set DeletionStatusID = (select StatusID from [Deletion] where Value = @Deletion)
where Name = @AssistantName
and IDNumber = @IDNumber", // 4
            @"select AuditID, IPAddress, E.Value, M.Value, SC.Value, DateOccured, [Parameters] from AuditHistory AH
join [Endpoint] E on AH.EndpointID = E.EndpointID
join Methods M on AH.MethodID = M.MethodID
join StatusCode SC on AH.StatusID = SC.StatusID", // 5
            @"select count(*) from AuditHistory AH
join [Endpoint] E on AH.EndpointID = E.EndpointID
join Methods M on AH.MethodID = M.MethodID
join StatusCode SC on AH.StatusID = SC.StatusID", // 6
            @"select IPAddress, DateOccured, E.Value, Field, OldValue, NewValue from [Change] C
join [Endpoint] E on C.EndpointID = E.EndpointID
join AuditHistory AH on E.EndpointID = AH.EndpointID", // 7
        "" };
    }
}
