using System.Collections.Generic;
using System.Threading.Tasks;
using user_services;
using user_services.RequestModels;
using user_services.ViewModels;

namespace user_api.Services
{
    public interface IRoleApiClient
    {
        Task<RoleViewModel> GetRole(string id);
        Task<Pagination<RoleViewModel>> GetRolesPaging(string filter, int pageIndex, int pageSize);
        Task<bool> PostRole(RoleRequestModel request);
        Task<bool> PutRole(string id, RoleRequestModel request);
        Task<bool> DeleteRole(string id);
        Task<Pagination<ApiRoleViewModel>> GetRoleClaims(string id, string filter, int pageIndex, int pageSize);
        Task<bool> PostRoleClaims(string roleId, RoleClaimRequestModel request);
    }
}
