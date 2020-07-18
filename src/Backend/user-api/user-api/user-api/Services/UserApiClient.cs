using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using user_services;
using user_services.RequestModels;
using user_services.ViewModels;

namespace user_api.Services
{
    public class UserApiClient : BaseApiClient, IUserApiClient
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserApiClient(IHttpClientFactory httpClientFactory,
          IConfiguration configuration,
          IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, configuration, httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<bool> PutUserPassword(string id, UserPasswordRequestModel request)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);

            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsync($"/api/users/{id}/change-password", data);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteUser(string id)
        {
            return await DeleteAsync($"/api/users/{id}", true);
        }

        public async Task<UserViewModel> GetById(string id)
        {
            return await GetAsync<UserViewModel>($"/api/users/{id}", true);
        }

        public async Task<List<UserRoleViewModel>> GetUserRoles(string id)
        {
            return await GetAsync<List<UserRoleViewModel>>($"/api/users/{id}/roles", true);
        }

        public async Task<Pagination<UserQuickViewModels>> GetUsersPaging(string filter, int pageIndex, int pageSize)
        {
            return await GetAsync<Pagination<UserQuickViewModels>>($"/api/users/filter?filter={filter}&pageIndex={pageIndex}&pageSize={pageSize}", true);
        }

        public async Task<bool> PostRolesToUser(string id, RoleAssignRequestModel request)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);

            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync($"/api/users/{id}/roles", data);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PostUser(UserRequestModel request)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);

            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync($"/api/users", data);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PutResetPassword(string id)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);

            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsync($"/api/users/{id}/reset-password", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PutUser(string id, UserRequestModel request)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);

            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsync($"/api/users/{id}", data);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveRolesFromUser(string id, RoleAssignRequestModel request)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);

            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsync($"/api/users/{id}/roles", data);
            return response.IsSuccessStatusCode;
        }
    }
}
