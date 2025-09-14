using B2b.Infrastructure.Service;
using B2b.Web.Middleware.JwtAuth;
using Microsoft.AspNetCore.Mvc;
using B2b.Dal.Entity;

namespace B2b.Web.Controllers.Dashboard
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class RoleApiController : ControllerBase
    {
        private readonly IRoleService _service;
        public RoleApiController(IRoleService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var plugins = _service.GetRolePermissions();
            return Ok(new { data =  _service.GetAll() , plugins, main= plugins.Where(p=>p.MainSecurityObjectId==0).OrderBy(p => p.Queno).ToList(), state = true });
        }
        [HttpPost]
        public IActionResult FindRole(Role data)
        {
            return Ok(new { dataRoles = _service.GetById(data.RoleId),data=_service.FindRolePermission(data),state=true });
        }
        [HttpPost]
        public IActionResult Add(Role data)
        {
            _service.AddAsync(data);
            return Ok(new { state = true });
        }
        [HttpPost]
        public IActionResult Remove(int id)
        {
            _service.Remove(id);
            return Ok(new {state=true});
        }
        [HttpPut]
        public IActionResult Update(Role data)
        {
            _service.Update(data);
            return Ok(new { state = true });
        }

    }
}
