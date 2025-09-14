using Microsoft.AspNetCore.Mvc;
using B2b.Web.Middleware.JwtAuth;
using System.Net.Http.Headers;

namespace B2b.Web.Controllers.Api
{
    [ApiController]
    [Authorize]
    public class FileApiController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        [Route("upload/upload-file")]
        [HttpPost, DisableRequestSizeLimit]
        public IActionResult UploadFileImg()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("uploads");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderName);
                if (file.Length > 0)
                {
                    string id = Guid.NewGuid().ToString();
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, id + "-" + fileName);
                    var dbPath = Path.Combine(folderName, id + "-" + fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    return Ok(new { Img = "/" + dbPath.Replace("\\", "/"), state = true });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [Route("upload/delete-file")]
        [HttpPost]
        public IActionResult DeleteFile([FromBody]string value)
        {
            value = value.Replace("/uploads/", "");
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","uploads", value);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                return Ok("Dosya silindi.");
            }
            return NotFound("Dosya bulunamadı.");
        }
    }
}
