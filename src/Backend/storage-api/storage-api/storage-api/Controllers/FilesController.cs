﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using storage_api.services;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace storage_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IStorageService _storageService;
        public FilesController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [AllowAnonymous]
        [HttpPost("upload"), DisableRequestSizeLimit]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file != null)
            {
                var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var fileName = $"{originalFileName.Substring(0, originalFileName.LastIndexOf('.'))}{Path.GetExtension(originalFileName)}";
                await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);

                var fileEntity = new File()
                {
                    FileName = fileName,
                    FilePath = _storageService.GetFileUrl(fileName),
                    FileSize = file.Length,
                    FileType = Path.GetExtension(fileName)
                };
                return Ok(fileEntity);
            }
            return BadRequest();

        }

    }
}
