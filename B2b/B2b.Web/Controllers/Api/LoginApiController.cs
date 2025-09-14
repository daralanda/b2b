using Microsoft.AspNetCore.Mvc;
using B2b.Infrastructure.RequestDto;
using B2b.Infrastructure.Service.Authorization;
using B2b.Infrastructure.Service.MainService;
using B2b.Infrastructure.Service.UserService;
using B2b.Infrastructure.Service;
using B2b.Web.Middleware.JwtAuth;
using NuGet.Protocol;

namespace B2b.Web.Controllers.Api
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class LoginApiController(IJwtUtils jwtUtils, IMainService mainService, IUserService userService, IRoleService roleService) : ControllerBase
    {

        private readonly IJwtUtils _jwtUtils = jwtUtils;
        private readonly IUserService _userService = userService;
        private readonly IMainService _mainService = mainService;
        private readonly IRoleService _roleService = roleService;

        [AllowAnonymous] //  actiona izin vermek icin
        [HttpPost]
        public IActionResult Login(AuthenticateRequest model)
        {
            var response = _jwtUtils.Authenticate(model);
            if (response == null)
                return BadRequest(new { message = "Kullanıcı Adı yada şifre yanlış", state = false });
            else
            {
                var role = _roleService.GetById(response.RoleId);
                if (role != null )
                {
                    HttpContext.Session.SetString("User", response.ToJson());
                    HttpContext.Session.SetString("UserId", response.UserId.ToString());
                    HttpContext.Session.SetString("RoleId", response.RoleId.ToString());
                    HttpContext.Session.SetString("FullName", response.FirstName + " " + response.LastName);
                    return Ok(new { data = response, state = true, url = "/" });
                }
                else
                {
                    return BadRequest(new { message = "Kullanıcı Adı yada şifre yanlış", state = false });
                }
            }
        }
        [HttpGet]
        public IActionResult LogOut()
        {
            HttpContext.Items.Clear();
            HttpContext.Session.Clear();
            return Ok(new { state = true });
        }

    }
}
