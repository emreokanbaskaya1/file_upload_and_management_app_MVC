using Microsoft.AspNetCore.Mvc;
using proje_okan.Models;

namespace proje_okan.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public FilesController(IWebHostEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }

        // Dosya Yükleme
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] UploadModel model)
        {
            var file = model.File;

            if (file == null || file.Length == 0)
                return BadRequest("Dosya seçilmedi.");

            var allowedExtensions = _config.GetSection("FileSettings:AllowedExtensions").Get<string[]>();
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Yalnızca PDF, PNG, JPG dosyalar yüklenebilir.");

            var uploadFolder = _config["FileSettings:UploadFolder"];
            var uploadPath = Path.Combine(_env.ContentRootPath, uploadFolder);

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { message = "Dosya yüklendi", fileName = file.FileName });
        }

        // Dosya Listeleme
        [HttpGet]
        public IActionResult GetFiles()
        {
            var uploadFolder = _config["FileSettings:UploadFolder"];
            var uploadPath = Path.Combine(_env.ContentRootPath, uploadFolder);

            if (!Directory.Exists(uploadPath))
                return Ok(new List<string>());

            var files = Directory.GetFiles(uploadPath)
                                 .Select(Path.GetFileName)
                                 .ToList();

            return Ok(files);
        }


        // Dosya Silme
        [HttpDelete("{filename}")]
        public IActionResult DeleteFile(string filename)
        {
            var uploadFolder = _config["FileSettings:UploadFolder"];
            var uploadPath = Path.Combine(_env.ContentRootPath, uploadFolder);
            var filePath = Path.Combine(uploadPath, filename);

            if (!System.IO.File.Exists(filePath))
                return NotFound("Dosya bulunamadı.");

            System.IO.File.Delete(filePath);
            return Ok("Dosya silindi.");
        }
    }
}
