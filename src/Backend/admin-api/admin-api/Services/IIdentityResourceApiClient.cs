using admin_services.RequestModels;
using System.Threading.Tasks;

namespace admin_api.Services
{
    public interface IIdentityResourceApiClient
    {
        Task<bool> PostIdentityResource(IdentityResourceRequestModel request);

        Task<bool> DeleteIdentityResource(string identityResourceName);
    }
}
