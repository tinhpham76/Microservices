namespace auth_server.Data.Entities
{
    public class ApiResourceProperty : Property
    {
        public int ApiResourceId { get; set; }
        public ApiResource ApiResource { get; set; }
    }
}
