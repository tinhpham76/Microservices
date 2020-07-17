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
    public class ClientApiClient : BaseApiClient, IClientApiClient
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientApiClient(IHttpClientFactory httpClientFactory,
          IConfiguration configuration,
          IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, configuration, httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<bool> DeleteClient(string ClientId)
        {
            return await DeleteAsync($"/api/clients/{ClientId}", true);
        }

        public async Task<bool> PostClient(ClientRequestModel request)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);

            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync($"/api/clients", data);
            return response.IsSuccessStatusCode;
        }
    }
}
