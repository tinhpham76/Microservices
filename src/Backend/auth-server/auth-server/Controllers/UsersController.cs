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
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, request);
            }
            return BadRequest();
        }


        //Find user with User Name, Email, First Name, Last Name, Phone Number 
        [HttpGet("filter")]
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
                    UserName = x.UserName,
                    FullName = x.LastName + ' ' + x.FirstName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    Dob = x.Dob.ToString("yyyy/MM/dd")
                }).ToListAsync();

            var pagination = new Pagination<UserQuickViewModels>
            {
                Items = items,
                TotalRecords = totalReconds
            };
            return Ok(pagination);
        }

        //Get detail user with user id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
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
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, [FromBody] UserRequestModel request)
        {
            var user = await _userManager.FindByIdAsync(id);
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
        [HttpPut("{id}/reset-password")]
        public async Task<IActionResult> PutResetPassword(string id)
        {

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound($"Cannot found user with id: {id}");
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
        [HttpPut("{id}/change-password")]
        public async Task<IActionResult> PutUserPassword(string id, [FromBody] UserPasswordRequestModel request)
        {
            if (request.CheckPassword != request.NewPassword)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound($"Cannot found user with id: {id}");

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (result.Succeeded)
            {
                return NoContent();
            }
            return BadRequest();
        }

        //Delete user with user id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var adminUsers = await _userManager.GetUsersInRoleAsync(SystemConstants.Roles.Admin);
            var otherUsers = adminUsers.Where(x => x.Id != id).ToList();
            if (otherUsers.Count == 0)
            {
                return BadRequest("You cannot remove the only admin user remaining.");
            }
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                var uservm = new UserViewModel()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Dob = user.Dob.ToString("yyyy/MM/dd"),
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreateDate = user.CreateDate.ToString("yyyy/MM/dd"),
                    LastModifiedDate = user.LastModifiedDate.ToString().Substring(0, 10)
                };
                return Ok(uservm);
            }
            return BadRequest();

        }

        [HttpGet("{userId}/roles")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Count == 0)
            {
                var dataRole = await _roleManager.Roles.Select(x => new UserRoleViewModel()
                {
                    Label = x.Id,
                    Value = x.Id,
                    Checked = userRoles.Contains(x.Id) ? true : false,
                    Disabled = false,
                    Name = x.Name
                }).ToListAsync();
                return Ok(dataRole);
            }
            var dataRoles = await _roleManager.Roles.Select(x => new UserRoleViewModel()
            {
                Label = x.Id,
                Value = x.Id,
                Checked = userRoles.Contains(x.Id) ? true : false,
                Disabled = userRoles.Contains(x.Id) ? false : true,
                Name = x.Name
            }).ToListAsync();
            return Ok(dataRoles);
        }

        [HttpPost("{userId}/roles")]
        public async Task<IActionResult> PostRolesToUser(string userId, [FromBody] RoleAssignRequestModel request)
        {
            if (request.RoleNames?.Length == 0)
            {
                return BadRequest("Role names cannot empty");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound($"Cannot found user with id: {userId}");
            var result = await _userManager.AddToRolesAsync(user, request.RoleNames);
            if (result.Succeeded)
                return Ok();

            return BadRequest(result);
        }

        [HttpPut("{userId}/roles")]
        public async Task<IActionResult> RemoveRolesFromUser(string userId, [FromBody] RoleAssignRequestModel request)
        {
            if (request.RoleNames?.Length == 0)
            {
                return BadRequest();
            }
            if (request.RoleNames.Length == 1 && request.RoleNames[0] == SystemConstants.Roles.Admin)
            {
                return base.BadRequest();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();
            var result = await _userManager.RemoveFromRolesAsync(user, request.RoleNames);
            if (result.Succeeded)
                return Ok();

            return BadRequest();
        }
        #endregion
    }
}
