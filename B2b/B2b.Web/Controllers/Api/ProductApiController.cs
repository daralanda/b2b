using B2b.Dal.Entity;
using B2b.Infrastructure.Service.ProductService;
using B2b.Web.Middleware.JwtAuth;
using Microsoft.AspNetCore.Mvc;

namespace B2b.Web.Controllers.Api
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class ProductApiController : ControllerBase
    {
        private readonly IProductService _service;
        public ProductApiController(IProductService service)
        {
            _service = service;
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
        public IActionResult GetPrice(int id)
        {
            return Ok(_service.GetPrice(id));
        }
    }
}