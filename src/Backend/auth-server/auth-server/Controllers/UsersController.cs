using auth_server.Authorization;
using auth_server.Data.Entities;
using auth_services;
using auth_services.Constants;
using auth_services.RequestModel;
using auth_services.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace auth_server.Controllers
{
    public class UsersController : BaseController
    {
        #region User
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UsersController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //Post new user
        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] UserRequestModel request)
        {
            var user = new User()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = request.UserName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Dob = DateTime.Parse(request.Dob),
                CreateDate = DateTime.UtcNow

            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                var useRole = await _userManager.FindByNameAsync(request.UserName);
                await _userManager.AddToRoleAsync(useRole, "Member");
                return Ok();
            }
            return BadRequest();
        }


        //Find user with User Name, Email, First Name, Last Name, Phone Number 
        [HttpGet("filter")]
        [ClaimRequirementAttribute(PermissionCode.VIEW)]
        public async Task<IActionResult> GetUsersPaging(string filter, int pageIndex, int pageSize)
        {
            var query = _userManager.Users;
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.Email.Contains(filter)
                || x.UserName.Contains(filter)
                || x.PhoneNumber.Contains(filter)
                || x.FirstName.Contains(filter)
                || x.LastName.Contains(filter));
            }
            var totalReconds = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new UserQuickViewModels()
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    FullName = x.LastName + ' ' + x.FirstName,
                    Email = x.Email,
                }).ToListAsync();

            var pagination = new Pagination<UserQuickViewModels>
            {
                Items = items,
                TotalRecords = totalReconds
            };
            return Ok(pagination);
        }

        //Get detail user with user id
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var userViewModel = new UserViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Dob = user.Dob.ToString("yyyy/MM/dd"),
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreateDate = user.CreateDate.ToString("yyyy/MM/dd"),
                LastModifiedDate = user.LastModifiedDate != null ? user.LastModifiedDate.ToString().Substring(0, 10) : null
            };
            return Ok(userViewModel);
        }

        //Put user wiht user id
        [HttpPut("{userId}")]
        public async Task<IActionResult> PutUser(string userId, [FromBody] UserRequestModel request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.Dob = DateTime.Parse(request.Dob);
            user.PhoneNumber = request.PhoneNumber;
            user.LastModifiedDate = DateTime.Now;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest();
        }

        //Put reset user password with user id
        [HttpPut("{userId}/reset-password")]
        public async Task<IActionResult> PutResetPassword(string userId)
        {

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound($"Cannot found user with id: {userId}");
            var newPassword = _userManager.PasswordHasher.HashPassword(user, "User@123");
            user.PasswordHash = newPassword;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return NoContent();
            }
            return BadRequest();
        }

        //Put user password with user id
        [HttpPut("{userId}/change-password")]
        public async Task<IActionResult> PutUserPassword(string userId, [FromBody] UserPasswordRequestModel request)
        {
            if (request.CheckPassword != request.NewPassword)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound($"Cannot found user with id: {userId}");

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (result.Succeeded)
            {
                return NoContent();
            }
            return BadRequest();
        }

        //Delete user with user id
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var adminUsers = await _userManager.GetUsersInRoleAsync(SystemConstants.Roles.Admin);
            var otherUsers = adminUsers.Where(x => x.Id != userId).ToList();
            if (otherUsers.Count == 0)
            {
                return BadRequest();
            }
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest();

        }

        [HttpGet("{userId}/userRoles")]
        public async Task<IActionResult> GetUserDetailWithRoles(string userId)
        {

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            var roles = await _roleManager.Roles.Select(x => x.Name.ToString()).ToListAsync();
            var roleTemps = roles.Select(x => new UserRoleTempViewModels()
            {
                Label = x,
                Value = x
            }).ToList();

            var userRoleViewModel = new UserRoleViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Dob = user.Dob.ToString("yyyy/MM/dd"),
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreateDate = user.CreateDate.ToString("yyyy/MM/dd"),
                LastModifiedDate = user.LastModifiedDate != null ? user.LastModifiedDate.ToString().Substring(0, 10) : null,
                UserRoles = userRoles.ToList(),
                Roles = roleTemps
            };
            return Ok(userRoleViewModel);
        }

        [HttpPut("{userId}/userRoles")]
        public async Task<IActionResult> PutUserDetailWithRoles(string userId, [FromBody] UserRoleRequestModel request)
        {
            if (request.UserRoles.Count == 0)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.Dob = DateTime.Parse(request.Dob);
            user.PhoneNumber = request.PhoneNumber;
            user.LastModifiedDate = DateTime.Now;
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                if (!(request.UserRoles.Contains(userRole)))
                {
                    await _userManager.RemoveFromRoleAsync(user, userRole);
                }
            }

            foreach (var requestRole in request.UserRoles)
            {
                if (!userRoles.Contains(requestRole))
                {
                    await _userManager.AddToRoleAsync(user, requestRole);
                }
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest();


        }
        #endregion
    }
}
