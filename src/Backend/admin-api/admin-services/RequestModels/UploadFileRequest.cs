using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace admin_services.RequestModels
{
    public class UploadFileRequest
    {
        public List<IFormFile> Files { get; set; }
    }
}
