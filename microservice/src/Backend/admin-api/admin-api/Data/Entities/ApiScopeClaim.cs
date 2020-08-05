namespace admin_api.Data.Entities
{
    public class ApiScopeClaim : UserClaim
    {
        public int ScopeId { get; set; }
        public ApiScope Scope { get; set; }
    }
}
