using System.Configuration;

namespace Identity.Dapper
{
    public class ApplicationConfiguration
    {
        /// <summary>
        /// Get the connection string.
        /// <exception cref="ConfigurationErrorsException"></exception>
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["identity"].ConnectionString;
            }
        }
    }
}
