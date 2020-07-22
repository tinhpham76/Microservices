using System.Collections.Generic;
using System.Threading.Tasks;
using user_services;
using user_services.RequestModels;
using user_services.ViewModels;

namespace user_api.Services
{
    public interface IUserApiClient
    {
        Task<bool> PostUser(UserRequestModel request);
        Task<Pagination<UserQuickViewModels>> GetUsersPaging(string filter, int pageIndex, int pageSize);
        Task<UserViewModel> GetById(string id);
        Task<bool> PutUser(string id, UserRequestModel request);
        Task<bool> PutResetPassword(string id);
        Task<bool> PutUserPassword(string id, UserPasswordRequestModel request);
        Task<bool> DeleteUser(string id);
        Task<UserRoleViewModel> GetUserDetailWithRoles(string id);
        Task<bool> PutUserDetailWithRoles(string id, UserRoleRequestModel request);

    }
}
