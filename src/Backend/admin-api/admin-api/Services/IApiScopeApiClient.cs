using admin_services.RequestModels;
using System.Threading.Tasks;

namespace admin_api.Services
{
    public interface IApiScopeApiClient
    {
        Task<bool> PostApiScope(ApiScopeRequestModel request);

        Task<bool> DeleteApiScope(string apiScopeName);
    }
}
