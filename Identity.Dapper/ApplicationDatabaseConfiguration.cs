namespace Identity.Dapper
{
    public class ApplicationDatabaseConfiguration : IApplicationDatabaseConfiguration
    {
        public string GetConnectionString()
        {
            return ApplicationConfiguration.ConnectionString;
        }
    }
}