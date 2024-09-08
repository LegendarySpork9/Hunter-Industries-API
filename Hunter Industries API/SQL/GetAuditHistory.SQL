select AuditID, IPAddress, E.Value, M.Value, SC.Value, DateOccured, [Parameters] from AuditHistory AH
join [Endpoint] E on AH.EndpointID = E.EndpointID
join Methods M on AH.MethodID = M.MethodID
join StatusCode SC on AH.StatusID = SC.StatusID
where AH.AuditID is not null