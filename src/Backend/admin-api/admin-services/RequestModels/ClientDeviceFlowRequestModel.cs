using System;
using System.Collections.Generic;
using System.Text;

namespace admin_services.RequestModels
{
    public class ClientDeviceFlowRequestModel
    {
        public string UserCodeType { get; set; }
        public int DeviceCodeLifetime { get; set; }
    }
}
