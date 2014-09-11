using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Identity.Dapper.Models;
using Identity.Dapper.TsqlQueries;

namespace Identity.Dapper.Stores
{
    public class RoleStore
    {
        private readonly IApplicationDatabaseConfiguration applicationDatabaseConfiguration;

        public RoleStore(IApplicationDatabaseConfiguration applicationDatabaseConfiguration)
        {
            this.applicationDatabaseConfiguration = applicationDatabaseConfiguration;
        }

        /// <summary>
        /// Create a role
        /// </summary>
        /// <param name="role">The role to create</param>
        public async Task Insert(Role role)
        {
            using (var sqlConnection = new SqlConnection(applicationDatabaseConfiguration.GetConnectionString()))
            {
                await sqlConnection.OpenAsync();

                var id = await sqlConnection.QueryAsync<int>(RoleTsql.Insert, role);
                role.Id = id.Single();
            }
        }

        /// <summary>
        /// Get all the roles
        /// </summary>
        /// <returns>A list of role names</returns>
        public async Task<List<Role>> GetRoles()
        {
            using (var sqlConnection = new SqlConnection(applicationDatabaseConfiguration.GetConnectionString()))
            {
                await sqlConnection.OpenAsync();

                var roleEntities = await sqlConnection.QueryAsync<Role>(RoleTsql.GetAll);

                return roleEntities.ToList();
            }
        }
    }
}
