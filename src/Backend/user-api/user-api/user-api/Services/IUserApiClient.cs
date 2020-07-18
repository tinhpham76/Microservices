using System;
using System.Collections.Generic;
using System.Linq;
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
        Task<List<UserRoleViewModel>> GetUserRoles(string id);
        Task<bool> PostRolesToUser(string id, RoleAssignRequestModel request);
        Task<bool> RemoveRolesFromUser(string id, RoleAssignRequestModel request);


    }
}
