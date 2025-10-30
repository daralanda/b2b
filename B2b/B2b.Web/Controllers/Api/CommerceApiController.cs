using Azure;
using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;
using B2b.Infrastructure.Service.BannerService;
using B2b.Infrastructure.Service.CommerceService;
using B2b.Web.Middleware.JwtAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace B2b.Web.Controllers.Api
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class CommerceApiController : ControllerBase
    {
        private readonly ICommerceService _service;
        public CommerceApiController(ICommerceService service)
        {
            _service = service;
        }
        [HttpGet]
        public IActionResult GetProducts()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            var data = _service.GetProducts(userId);
            return Ok(data);
        }
        [HttpGet]
        public IActionResult GetCategories()
        {
            var data = _service.GetCategories();
            return Ok(data);
        }
        [HttpGet]
        public IActionResult GetProduct(int ProductId)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var data = _service.GetProduct(ProductId, userId);
            return Ok(data);
        }
        [HttpGet]
        public IActionResult GetProductImage(int ProductId)
        {
            var data = _service.GetProductImage(ProductId);
            return Ok(data);
        }
        [HttpGet]
        public IActionResult GetCampaignProducts()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var data = _service.GetCampaignProducts(userId);
            return Ok(data);
        }

        [HttpGet]
        public IActionResult GetCampaignProductList()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var data = _service.GetCampaignProductList(userId);
            return Ok(data);
        }

        [HttpGet]
        public IActionResult GetLatestProducts()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var data = _service.GetLatestProducts(userId);
            return Ok(data);
        }
        [HttpGet]
        public IActionResult GetProductInfo(int Id)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            return Ok(_service.GetProductInfo(userId,Id));
        }
        [HttpGet]
        public IActionResult GetAllOrder()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            return Ok(_service.GetAllOrder(userId));
        }
        [HttpGet]
        public IActionResult GetByOrderId(int id)
        {
            return Ok(_service.GetByOrderId(id));
        }
       
    }
}
