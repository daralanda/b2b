using B2b.Dal.Entity;
using B2b.Infrastructure.Service.BannerService;
using B2b.Infrastructure.Service.CartService;
using B2b.Web.Middleware.JwtAuth;
using Microsoft.AspNetCore.Mvc;

namespace B2b.Web.Controllers.Api
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class CartApiController : ControllerBase
    {
        private readonly ICartService _service;
        public CartApiController(ICartService service)
        {
            _service = service;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            return Ok(_service.GetAll(userId));
        }
     
        [HttpPost]
        public IActionResult Add(Cart data)
        {

            data.UserId = int.Parse(HttpContext.Session.GetString("UserId"));
            data.CreatedDate = DateTime.Now;
            return Ok(_service.Add(data));
        }
        [HttpGet]
        public IActionResult Remove(int id)
        {
            return Ok(_service.Remove(id));
        }
        [HttpPut]
        public IActionResult Update(Cart data)
        {

            return Ok(_service.Update(data));
        }

    }
}
