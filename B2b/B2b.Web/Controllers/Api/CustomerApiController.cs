using B2b.Dal.Entity;
using B2b.Infrastructure.Service.CustomerService;
using B2b.Web.Middleware.JwtAuth;
using Microsoft.AspNetCore.Mvc;

namespace B2b.Web.Controllers.Api
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class CustomerApiController : ControllerBase
    {
        private readonly ICustomerService _service;
        public CustomerApiController(ICustomerService service)
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
        public IActionResult Add(Customer data)
        {
            return Ok(_service.Add(data));
        }
        [HttpGet]
        public IActionResult Remove(int id)
        {
            return Ok(_service.Remove(id));
        }
        [HttpPut]
        public IActionResult Update(Customer data)
        {
            return Ok(_service.Update(data));
        }
        [HttpGet]
        public IActionResult GetAllCities()
        {
            return Ok(_service.GetAllCities());
        }
        [HttpGet]
        public IActionResult GetAllDistricts()
        {
            return Ok(_service.GetAllDistricts());
        }

    }
}