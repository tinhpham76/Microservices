namespace auth_services.RequestModel
{
    public class RoleClaimRequestModel
    {
        public string Type { get; set; }
        public bool View { get; set; }
        public bool Create { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
    }
}
