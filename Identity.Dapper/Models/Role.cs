using Microsoft.AspNet.Identity;

namespace Identity.Dapper.Models
{
    public class Role : IRole<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
