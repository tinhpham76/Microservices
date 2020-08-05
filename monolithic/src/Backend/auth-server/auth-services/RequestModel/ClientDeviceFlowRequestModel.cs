namespace auth_services.RequestModel
{
    public class ClientDeviceFlowRequestModel
    {
        public string UserCodeType { get; set; }
        public int DeviceCodeLifetime { get; set; }
    }
}
