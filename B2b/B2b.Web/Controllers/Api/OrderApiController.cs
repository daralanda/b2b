using B2b.Dal.Entity;
using B2b.Infrastructure.Service.OrderService;
using B2b.Web.Middleware.JwtAuth;
using Microsoft.AspNetCore.Mvc;

namespace B2b.Web.Controllers.Api
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class OrderApiController : ControllerBase
    {
        private readonly IOrderService _service;
        public OrderApiController(IOrderService service)
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
        public IActionResult Add(Order data)
        {
            return Ok(_service.Add(data));
        }
        [HttpGet]
        public IActionResult Remove(int id)
        {
            return Ok(_service.Remove(id));
        }
        [HttpPut]
        public IActionResult Update(Order data)
        {
            return Ok(_service.Update(data));
        }
        [HttpPost]
        public IActionResult CreateOrder(Infrastructure.RequestDto.OrderDto dto)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            return Ok(_service.CreateOrder(dto, userId));
        }
    }
}
