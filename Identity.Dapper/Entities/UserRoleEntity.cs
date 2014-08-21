namespace Identity.Dapper.Entities
{
    public class UserRoleEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}