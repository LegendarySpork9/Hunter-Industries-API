using System.Data.SqlClient;
using HunterIndustriesAPI.Models;

namespace HunterIndustriesAPI.Services.Assistant
{
    public class ConfigService
    {
        // Gets the config(s) from the database.
        public (string[], string[], string[], string[], bool[], string[], int, string) GetAssistantConfig(string assistantName, string assistantID)
        {
            try
            {
                // Make into Object
                string[] AssistantNames = Array.Empty<string>();
                string[] AssistantIDs = Array.Empty<string>();
                string[] UserNames = Array.Empty<string>();
                string[] HostNames = Array.Empty<string>();
                bool[] Deletions = Array.Empty<bool>();
                string[] Versions = Array.Empty<string>();

                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;
                SqlDataReader dataReader;

                // Obtaines and returns all the rows in the AssistantInformation table.
                string sqlQuery = @"select AI.Name, IDNumber, U.Name, L.HostName, D.Value, V.Value from AssistantInformation AI
join [Location] L on AI.LocationID = L.LocationID
join Deletion D on AI.DeletionStatusID = D.StatusID
join [Version] V on AI.VersionID = V.VersionID
join [User] U on AI.UserID = U.UserID
where AI.Name is not null";

                if (!string.IsNullOrEmpty(assistantName))
                {
                    sqlQuery += "\nand AI.Name = @AssistantName";
                }

                if (!string.IsNullOrEmpty(assistantID))
                {
                    sqlQuery += "\nand AI.IDNumber = @AssistantID";
                }

                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);

                if (sqlQuery.Contains("@AssistantName"))
                {
                    command.Parameters.Add(new SqlParameter("@AssistantName", assistantName));
                }

                if (sqlQuery.Contains("@AssistantID"))
                {
                    command.Parameters.Add(new SqlParameter("@AssistantID", assistantID));
                }

                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    AssistantNames = AssistantNames.Append(dataReader.GetString(0)).ToArray();
                    AssistantIDs = AssistantIDs.Append(dataReader.GetString(1)).ToArray();
                    UserNames = UserNames.Append(dataReader.GetString(2)).ToArray();
                    HostNames = HostNames.Append(dataReader.GetString(3)).ToArray();
                    Deletions = Deletions.Append(Convert.ToBoolean(dataReader.GetString(4))).ToArray();
                    Versions = Versions.Append(dataReader.GetString(5)).ToArray();
                }

                dataReader.Close();
                connection.Close();

                return (AssistantNames, AssistantIDs, UserNames, HostNames, Deletions, Versions, GetTotalConfigs(command), GetMostRecentVersion());
            }

            catch (Exception ex)
            {
                return (Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), Array.Empty<bool>(), Array.Empty<string>(), 0, string.Empty);
            }
        }

        // Gets the total number of records in the AssistantInformation table.
        private int GetTotalConfigs(SqlCommand command)
        {
            try
            {
                int totalRecords = 0;

                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlDataReader dataReader;

                // Obtaines and returns the number of rows in the AuditHistory table.
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command.Connection = connection;
                command.CommandText = command.CommandText.Replace(@"select AI.Name, IDNumber, U.Name, L.HostName, D.Value, V.Value from AssistantInformation AI
join [Location] L on AI.LocationID = L.LocationID
join Deletion D on AI.DeletionStatusID = D.StatusID
join [Version] V on AI.VersionID = V.VersionID
join [User] U on AI.UserID = U.UserID", "select count(*) from AssistantInformation AI");
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    totalRecords = dataReader.GetInt32(0);
                }

                dataReader.Close();
                connection.Close();

                return totalRecords;
            }

            catch (Exception ex)
            {
                return 0;
            }
        }

        // Gets the most recent release version in the Versions table.
        private string GetMostRecentVersion()
        {
            try
            {
                string version = string.Empty;

                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;
                SqlDataReader dataReader;

                // Obtaines and returns the latest version.
                string sqlQuery = @"select top 1 Value from [Version]
order by VersionID desc";

                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    version = dataReader.GetString(0);
                }

                dataReader.Close();
                connection.Close();

                return version;
            }

            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        // Checks whether the given assistant already exists in the table.
        public bool AssistantExists(string assistantName, string assistantID)
        {
            try
            {
                string? assistant = null;

                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;
                SqlDataReader dataReader;

                // Obtaines and returns all the rows in the AssistantInformation table.
                string sqlQuery = @"select AI.Name from AssistantInformation AI
join [Location] L on AI.LocationID = L.LocationID
join Deletion D on AI.DeletionStatusID = D.StatusID
join [Version] V on AI.VersionID = V.VersionID
join [User] U on AI.UserID = U.UserID
where AI.Name = @AssistantName
and AI.IDNumber = @AssistantID";

                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@AssistantName", assistantName));
                command.Parameters.Add(new SqlParameter("@AssistantID", assistantID));
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    assistant = dataReader.GetString(0);
                }

                dataReader.Close();
                connection.Close();

                if (string.IsNullOrEmpty(assistant))
                {
                    return false;
                }

                return true;
            }

            catch (Exception ex)
            {
                return false;
            }
        }

        // Inserts the new config into the relevant tables.
        public bool AssistantConfigCreated(string assistantName, string idNumber, string assignedUser, string hostName)
        {
            try
            {
                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;

                // Inserts the values into the relevant tables to create the config.
                string sqlQuery = @"insert into [Location] (HostName, IPAddress)
output inserted.LocationID
values (@Hostname, @IPAddress)";
                int rowsAffected;

                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@Hostname", hostName));
                command.Parameters.Add(new SqlParameter("@IPAddress", "PlaceHolder"));
                var locationId = command.ExecuteScalar();

                if (locationId == null)
                {
                    connection.Close();
                    return false;
                }

                sqlQuery = @"insert into [User] (Name)
output inserted.UserID
values (@Name)";
                rowsAffected = 0;

                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@Name", assignedUser));
                var userId = command.ExecuteScalar();

                if (userId == null)
                {
                    connection.Close();
                    return false;
                }

                sqlQuery = @"insert into [AssistantInformation] (LocationID, DeletionStatusID, VersionID, UserID, Name, IDNumber)
values (@LocationID, 2, (select top 1 VersionID from [Version] order by VersionID desc), @UserID, @AssistantName, @IDNumber)";
                rowsAffected = 0;

                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@LocationID", locationId));
                command.Parameters.Add(new SqlParameter("@UserID", userId));
                command.Parameters.Add(new SqlParameter("@AssistantName", assistantName));
                command.Parameters.Add(new SqlParameter("@IDNumber", idNumber));
                rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                {
                    connection.Close();
                    return false;
                }

                connection.Close();
                return true;
            }

            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
