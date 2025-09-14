using B2b.Dal.Entity;
using B2b.Infrastructure.Service.BrandService;
using B2b.Infrastructure.Service.SliderService;
using B2b.Web.Middleware.JwtAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace B2b.Web.Controllers.Api
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class SliderApiController : ControllerBase
    {
        private readonly ISliderService _service;
        public SliderApiController(ISliderService service)
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
        public IActionResult Add(Slider data)
        {
            return Ok(_service.Add(data));
        }
        [HttpGet]
        public IActionResult Remove(int id)
        {
            return Ok(_service.Remove(id));
        }
        [HttpPut]
        public IActionResult Update(Slider data)
        {
            return Ok(_service.Update(data));
        }

    }
}
