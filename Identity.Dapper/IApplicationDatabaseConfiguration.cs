namespace Identity.Dapper
{
    public interface IApplicationDatabaseConfiguration
    {
        string GetConnectionString();
    }
}