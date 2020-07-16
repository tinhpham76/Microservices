﻿using admin_services.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin_api.Services
{
    public interface IClientApiClient
    {        
        Task<bool> PostClient(ClientRequestModel request);
        
        Task<bool> DeleteClient(string ClientId);
    }
}
