using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Identity.Dapper.Entities;
using Identity.Dapper.TsqlQueries;

namespace Identity.Dapper
{
    public class RoleEntityStore
    {
        private readonly IApplicationDatabaseConfiguration applicationDatabaseConfiguration;

        public RoleEntityStore(IApplicationDatabaseConfiguration applicationDatabaseConfiguration)
        {
            this.applicationDatabaseConfiguration = applicationDatabaseConfiguration;
        }

        /// <summary>
        /// Create a role
        /// </summary>
        /// <param name="roleEntity">The role to create</param>
        public async Task Insert(RoleEntity roleEntity)
        {
            using (var sqlConnection = new SqlConnection(applicationDatabaseConfiguration.Get()))
            {
                await sqlConnection.OpenAsync();

                var id = await sqlConnection.QueryAsync<int>(RoleEntityTsql.Insert, roleEntity);
                roleEntity.Id = id.Single();
            }
        }

        /// <summary>
        /// Get all the roles
        /// </summary>
        /// <returns>A list of role names</returns>
        public async Task<List<RoleEntity>> Get()
        {
            using (var sqlConnection = new SqlConnection(applicationDatabaseConfiguration.Get()))
            {
                await sqlConnection.OpenAsync();

                var roleEntities = await sqlConnection.QueryAsync<RoleEntity>(RoleEntityTsql.GetAll);

                return roleEntities.ToList();
            }
        }
    }
}
