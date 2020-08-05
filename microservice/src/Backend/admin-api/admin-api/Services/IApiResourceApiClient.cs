using admin_services.RequestModels;
using System.Threading.Tasks;

namespace admin_api.Services
{
    public interface IApiResourceApiClient
    {
        Task<bool> PostApiResource(ApiResourceRequestModel request);

        Task<bool> DeleteApiResource(string apiResourceName);
    }
}
