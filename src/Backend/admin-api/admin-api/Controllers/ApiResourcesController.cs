using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using admin_api.Data;
using admin_api.Services;
using admin_services;
using admin_services.RequestModels;
using admin_services.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace admin_api.Controllers
{    
    public class ApiResourcesController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IApiResourceApiClient _apiResourceApiClient;
        public ApiResourcesController(ApplicationDbContext context, IApiResourceApiClient apiResourceApiClient)
        {
            _context = context;
            _apiResourceApiClient = apiResourceApiClient;
        }

        // Find api resource with name or display name
        [HttpGet("filter")]
        public async Task<IActionResult> GetApiResourcesPaging(string filter, int pageIndex, int pageSize)
        {
            var query = _context.ApiResources.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.Name.Contains(filter) || x.DisplayName.Contains(filter));

            }
            var totalReconds = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ApiResourceQuickViewModels()
                {
                    Id = x.Id,
                    Name = x.Name,
                    DisplayName = x.DisplayName,
                    Description = x.Description                  
                }).ToListAsync();

            var pagination = new Pagination<ApiResourceQuickViewModels>
            {
                Items = items,
                TotalRecords = totalReconds
            };
            return Ok(pagination);
        }

        [HttpPost]
        public async Task<IActionResult> PostApiResource([FromBody] ApiResourceRequestModel request)
        {
            var result = await _apiResourceApiClient.PostApiResource(request);
            if (result == true)
                return Ok($"Api Resource {request.Name} already created!");
            return BadRequest($"Api Resource {request.Name} can't created!");
        }

        [HttpDelete("{apiResourceName}")]
        public async Task<IActionResult> DeleteApiResource(string apiResourceName)
        {
            var result = await _apiResourceApiClient.DeleteApiResource(apiResourceName);
            if (result == true)
                return Ok($"Api Resource {apiResourceName} already deleted!");
            return BadRequest($"Api Resource {apiResourceName} can't deleted!");
        }
    }
}
