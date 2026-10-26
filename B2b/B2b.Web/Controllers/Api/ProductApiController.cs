using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;
using B2b.Infrastructure.Service.ProductService;
using B2b.Web.Middleware.JwtAuth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace B2b.Web.Controllers.Api
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class ProductApiController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IProductService _service;
        public ProductApiController(IProductService service, IWebHostEnvironment webHostEnvironment)
        {
            _service = service;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());
        }
        [HttpGet]
        public IActionResult GetById(int id)
        {
            return Ok(_service.GetById(id));
        }
        [HttpPost]
        public IActionResult Add(Product data)
        {
            return Ok(_service.Add(data));
        }
        [HttpGet]
        public IActionResult Remove(int id)
        {
            return Ok(_service.Remove(id));
        }
        [HttpPut]
        public IActionResult Update(Product data)
        {
            return Ok(_service.Update(data));
        }
        [HttpGet]
        public IActionResult GetImage(int id)
        {
            return Ok(_service.GetImage(id));
        }
        [HttpGet]
        public IActionResult GetImages()
        {
            return Ok(_service.GetImages());
        }
        [HttpGet]
        public IActionResult GetPrice(int id)
        {
            return Ok(_service.GetPrice(id));
        }
        [HttpPost]
        public IActionResult ProductAllSet(IFormFile file)
        {
            var invalid = ValidateExcelFile(file);
            if (invalid != null) return BadRequest(invalid);

            using var ms = new MemoryStream();
            file.CopyTo(ms);
            ms.Position = 0; // 
            var result = _service.ProductAllSet(ms);

            return Ok(result);
        }
        [HttpGet]
        public IActionResult GetTemplate()
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            string fileName = "UrunFiyatGuncelleme.xlsx";
            string fullPath = Path.Combine(uploadsFolder, fileName);
            var result = _service.GetTemplate(fullPath);
            return Ok(result);
        }
        [HttpPost]

        public IActionResult ProductPriceUpdateAll(IFormFile file)
        {
            var invalid = ValidateExcelFile(file);
            if (invalid != null) return BadRequest(invalid);

            using var ms = new MemoryStream();
            file.CopyTo(ms);
            ms.Position = 0; // 
            var result = _service.ProductPriceUpdateAll(ms);
            return Ok(result);
        }

        private static ResultDto<string> ValidateExcelFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return new ResultDto<string> { State = false, Message = "Dosya seçilmedi veya boş." };
            if (!string.Equals(Path.GetExtension(file.FileName), ".xlsx", StringComparison.OrdinalIgnoreCase))
                return new ResultDto<string> { State = false, Message = "Yalnızca .xlsx uzantılı Excel dosyası yüklenebilir." };
            return null;
        }
    }
}
