using admin_services.RequestModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace admin_api.Services
{
    public class ApiScopeApiClient : BaseApiClient, IApiScopeApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiScopeApiClient(IHttpClientFactory httpClientFactory,
          IConfiguration configuration,
          IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, configuration, httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

        }
        public async Task<bool> DeleteApiScope(string apiScopeName)
        {
            return await DeleteAsync($"/apiScopes/{apiScopeName}", true);
        }

        public async Task<bool> PostApiScope(ApiScopeRequestModel request)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);

            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync($"/ApiScopes", data);
            return response.IsSuccessStatusCode;
        }
    }
}
