using B2b.Infrastructure.Service.UserService;
using B2b.Infrastructure.Service;
using Microsoft.AspNetCore.Mvc;
using B2b.Web.Middleware.JwtAuth;
using B2b.Dal.Entity;

namespace B2b.Web.Controllers.Api
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class UserInfoApiController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IRoleService _roleService;
        public UserInfoApiController(IUserService service, IRoleService role)
        {
            _service = service;
            _roleService = role;
        }
        [HttpGet]
        public IActionResult GetById()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            return Ok(new { data = _service.GetById(userId), state = true,
                Cities = _service.GetCities(),
                Districts = _service.GetDistricts(),
            });
        }
        [HttpPost]
        public IActionResult Update(User data)
        {
            data.UserId  = int.Parse(HttpContext.Session.GetString("UserId"));
            HttpContext.Session.SetString("FullName", data.FirstName + " " + data.LastName);
            _service.UserUpdate(data);
            return Ok(new { state = true });
        }
    }
}
