namespace Identity.Dapper.Entities
{
    public class UserLoginEntity
    {
        public int Id { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public int UserId { get; set; }
    }
}