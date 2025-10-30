using B2b.Infrastructure.Service;
using B2b.Web.Middleware.JwtAuth;
using Microsoft.AspNetCore.Mvc;
using B2b.Dal.Entity;

using B2b.Infrastructure.Service.UserService;

namespace B2b.Web.Controllers.Api
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class UserApiController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IRoleService _roleService;
        public UserApiController(IUserService service, IRoleService role)
        {
            _service = service;
            _roleService = role;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(new { data = _service.GetAll(), roleData = _roleService.GetAll(), state = true });
        }
        [HttpGet]
        public IActionResult GetById()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            return Ok(new { data = _service.GetById(userId), state = true });
        }
        [HttpPost]
        public IActionResult Add(User data)
        {
            _service.Add(data);
            return Ok(new { state = true });
        }
        [HttpPost]
        public IActionResult Remove(int id)
        {
            _service.Remove(id);
            return Ok(new { state = true });
        }
        [HttpPost]
        public IActionResult Update(User data)
        {
            HttpContext.Session.SetString("FullName", data.FirstName + " " + data.LastName);
            _service.Update(data);
            return Ok(new { state = true });
        }

    }
}
