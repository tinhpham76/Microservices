using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using user_services;
using user_services.RequestModels;
using user_services.ViewModels;

namespace user_api.Services
{
    public class RoleApiClient : BaseApiClient, IRoleApiClient
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RoleApiClient(IHttpClientFactory httpClientFactory,
          IConfiguration configuration,
          IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, configuration, httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<bool> DeleteRole(string id)
        {
            return await DeleteAsync($"/roles/{id}", true);
        }

        public async Task<RoleViewModel> GetRole(string id)
        {
            return await GetAsync<RoleViewModel>($"/roles/{id}", true);
        }

        public async Task<Pagination<ApiRoleViewModel>> GetRoleClaims(string id, string filter, int pageIndex, int pageSize)
        {
            return await GetAsync<Pagination<ApiRoleViewModel>>($"/roles/{id}/claims/filter?filter={filter}&pageIndex={pageIndex}&pageSize={pageSize}", true);
        }

        public async Task<Pagination<ClientRoleViewModel>> GetClientClaims(string id, string filter, int pageIndex, int pageSize)
        {
            return await GetAsync<Pagination<ClientRoleViewModel>>($"/roles/{id}/clients/filter?filter={filter}&pageIndex={pageIndex}&pageSize={pageSize}", true);
        }

        public async Task<Pagination<RoleViewModel>> GetRolesPaging(string filter, int pageIndex, int pageSize)
        {
            return await GetAsync<Pagination<RoleViewModel>>($"/roles/filter?filter={filter}&pageIndex={pageIndex}&pageSize={pageSize}", true);
        }

        public async Task<bool> PostRole(RoleRequestModel request)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);

            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync($"/roles", data);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PostClientClaims(string roleId, ClientClaimRequestModel request)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);

            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync($"/roles/{roleId}/clients", data);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PostRoleClaims(string roleId, RoleClaimRequestModel request)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);

            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync($"/roles/{roleId}/claims", data);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PutRole(string id, RoleRequestModel request)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);

            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsync($"/roles/{id}", data);
            return response.IsSuccessStatusCode;
        }
    }
}
