using System.Data.SqlClient;
using Dapper;

namespace Identity.Dapper.IntegrationTests
{
    internal class Database
    {
        /// <summary>
        /// Remove all test data
        /// </summary>
        public static void TruncateAllTables()
        {
            var applicationDatabaseConfiguration = new ApplicationDatabaseConfiguration();
            using (var sqlConnection = new SqlConnection(applicationDatabaseConfiguration.GetConnectionString()))
            {
                sqlConnection.Open();

                sqlConnection.Execute("TRUNCATE TABLE [identity].[Role]");

                sqlConnection.Execute("TRUNCATE TABLE [identity].[User]");

                sqlConnection.Execute("TRUNCATE TABLE [identity].[UserClaim]");

                sqlConnection.Execute("TRUNCATE TABLE [identity].[UserLogin]");

                sqlConnection.Execute("TRUNCATE TABLE [identity].[UserRole]");

            }
        }
    }
}
