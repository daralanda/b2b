using B2b.Web.Middleware.JwtAuth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace B2b.Web.Controllers.Api
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class FileListApiController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public FileListApiController(IWebHostEnvironment env)
        {
            _env = env;
        }

        private string UploadsFolderPath => Path.Combine(_env.WebRootPath, "uploads");

        [HttpGet]
        public IActionResult GetFiles(string search = "", string sortBy = "Name", string order = "asc")
        {
            try
            {
                var folderPath = UploadsFolderPath;

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var directoryInfo = new DirectoryInfo(folderPath);
                var files = directoryInfo.GetFiles();

                var fileList = files.Select(f => new
                {
                    // Dosya adının ilk harfini büyük yap (TitleCase)
                    Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(f.Name.ToLower()),
                    Size = f.Length,
                    FormattedSize = FormatSize(f.Length),
                    Extension = f.Extension.ToUpper(),
                    CreatedDate = f.CreationTime.ToString("dd.MM.yyyy HH:mm"),
                    Url = $"{Request.Scheme}://{Request.Host}/uploads/{f.Name}"
                });

                // Arama Filtresi (Backend seviyesinde)
                if (!string.IsNullOrWhiteSpace(search))
                {
                    fileList = fileList.Where(f => f.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
                }

                // Sıralama
                fileList = (sortBy.ToLower(), order.ToLower()) switch
                {
                    ("size", "desc") => fileList.OrderByDescending(f => f.Size),
                    ("size", "asc") => fileList.OrderBy(f => f.Size),
                    ("date", "desc") => fileList.OrderByDescending(f => f.CreatedDate),
                    ("date", "asc") => fileList.OrderBy(f => f.CreatedDate),
                    ("name", "desc") => fileList.OrderByDescending(f => f.Name),
                    _ => fileList.OrderBy(f => f.Name)
                };

                return Ok(new { Success = true, Data = fileList.ToList() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult RenameFile([FromBody] RenameFileRequest model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.OldName) || string.IsNullOrWhiteSpace(model.NewName))
                    return BadRequest(new { Success = false, Message = "Geçersiz dosya adı." });

                var safeOldName = Path.GetFileName(model.OldName);
                var safeNewName = Path.GetFileName(model.NewName);

                var oldPath = Path.Combine(UploadsFolderPath, safeOldName);
                var newPath = Path.Combine(UploadsFolderPath, safeNewName);

                if (!System.IO.File.Exists(oldPath))
                    return NotFound(new { Success = false, Message = "Dosya bulunamadı." });

                if (System.IO.File.Exists(newPath))
                    return BadRequest(new { Success = false, Message = "Bu isimde bir dosya zaten var." });

                System.IO.File.Move(oldPath, newPath);

                return Ok(new { Success = true, Message = "Dosya adı başarıyla değiştirildi." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteFile([FromBody] DeleteFileRequest model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.FileName))
                    return BadRequest(new { Success = false, Message = "Dosya adı boş olamaz." });

                var safeName = Path.GetFileName(model.FileName);
                var filePath = Path.Combine(UploadsFolderPath, safeName);

                if (!System.IO.File.Exists(filePath))
                    return NotFound(new { Success = false, Message = "Dosya bulunamadı." });

                System.IO.File.Delete(filePath);

                return Ok(new { Success = true, Message = "Dosya başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        private static string FormatSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return $"{number:n2} {suffixes[counter]}";
        }
    }

    public class RenameFileRequest
    {
        public string OldName { get; set; }
        public string NewName { get; set; }
    }

    public class DeleteFileRequest
    {
        public string FileName { get; set; }
    }
}