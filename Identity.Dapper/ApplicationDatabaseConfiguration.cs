namespace Identity.Dapper
{
    public class ApplicationDatabaseConfiguration : IApplicationDatabaseConfiguration
    {
        public string Get()
        {
            return ApplicationConfiguration.ConnectionString;
        }
    }
}