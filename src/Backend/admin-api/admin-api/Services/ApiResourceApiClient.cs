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
    public class ApiResourceApiClient : BaseApiClient, IApiResourceApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiResourceApiClient(IHttpClientFactory httpClientFactory,
          IConfiguration configuration,
          IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, configuration, httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

        }
        public async Task<bool> DeleteApiResource(string apiResourceName)
        {
            return await DeleteAsync($"/apiResources/{apiResourceName}", true);
        }

        public async Task<bool> PostApiResource(ApiResourceRequestModel request)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);

            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync($"/ApiResources", data);
            return response.IsSuccessStatusCode;
        }
    }
}
