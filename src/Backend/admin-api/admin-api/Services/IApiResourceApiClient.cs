using admin_services.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin_api.Services
{
    public interface IApiResourceApiClient
    {
        Task<bool> PostApiResource(ApiResourceRequestModel request);

        Task<bool> DeleteApiResource(string apiResourceName);
    }
}
