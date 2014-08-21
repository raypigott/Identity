using Microsoft.AspNet.Identity;

namespace Identity.Dapper.Entities
{
    public class RoleEntity : IRole<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
