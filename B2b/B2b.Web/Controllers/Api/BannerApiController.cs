using B2b.Dal.Entity;
using B2b.Infrastructure.Service.BannerService;
using B2b.Web.Middleware.JwtAuth;
using Microsoft.AspNetCore.Mvc;

namespace B2b.Web.Controllers.Api
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class BannerApiController : ControllerBase
    {
        private readonly IBannerService _service;
        public BannerApiController(IBannerService service)
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
        public IActionResult Add(Banner data)
        {
            return Ok(_service.Add(data));
        }
        [HttpGet]
        public IActionResult Remove(int id)
        {
            return Ok(_service.Remove(id));
        }
        [HttpPut]
        public IActionResult Update(Banner data)
        {
            return Ok(_service.Update(data));
        }

    }
}